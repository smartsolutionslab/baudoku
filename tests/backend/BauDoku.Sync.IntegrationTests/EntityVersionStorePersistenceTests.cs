using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.Infrastructure.Persistence;
using BauDoku.Sync.Infrastructure.Persistence.Repositories;
using BauDoku.Sync.IntegrationTests.Fixtures;

namespace BauDoku.Sync.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class EntityVersionStorePersistenceTests
{
    private readonly PostgreSqlFixture fixture;

    public EntityVersionStorePersistenceTests(PostgreSqlFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task SetAndGetVersion_ShouldPersistAndRetrieve()
    {
        var entityId = Guid.NewGuid();

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, SyncVersion.From(1), """{"name":"Test"}""", DeviceIdentifier.From("device-001"), DeltaOperation.Create);
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var version = await store.GetCurrentVersionAsync(EntityType.Project, entityId);
            version.Value.Should().Be(1);
        }
    }

    [Fact]
    public async Task SetVersion_WhenEntryExists_ShouldUpdate()
    {
        var entityId = Guid.NewGuid();

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, SyncVersion.From(1), """{"v":1}""", DeviceIdentifier.From("device-001"), DeltaOperation.Create);
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, SyncVersion.From(2), """{"v":2}""", DeviceIdentifier.From("device-002"), DeltaOperation.Update);
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var version = await store.GetCurrentVersionAsync(EntityType.Project, entityId);
            version.Value.Should().Be(2);

            var payload = await store.GetCurrentPayloadAsync(EntityType.Project, entityId);
            payload.Should().Be("""{"v":2}""");
        }
    }

    [Fact]
    public async Task GetCurrentVersion_WhenNotExists_ShouldReturnInitial()
    {
        await using var context = fixture.CreateContext();
        var store = new EntityVersionStore(context);

        var version = await store.GetCurrentVersionAsync(EntityType.Project, Guid.NewGuid());

        version.Should().Be(SyncVersion.Initial);
        version.Value.Should().Be(0);
    }

    [Fact]
    public async Task GetCurrentPayload_WhenNotExists_ShouldReturnNull()
    {
        await using var context = fixture.CreateContext();
        var store = new EntityVersionStore(context);

        var payload = await store.GetCurrentPayloadAsync(EntityType.Project, Guid.NewGuid());

        payload.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentPayload_WhenExists_ShouldReturnPayload()
    {
        var entityId = Guid.NewGuid();

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Installation, entityId, SyncVersion.From(1), """{"data":"value"}""", DeviceIdentifier.From("device-001"), DeltaOperation.Create);
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var payload = await store.GetCurrentPayloadAsync(EntityType.Installation, entityId);
            payload.Should().Be("""{"data":"value"}""");
        }
    }

    [Fact]
    public async Task GetChangedSince_ShouldReturnRecentChanges()
    {
        var entityId = Guid.NewGuid();

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, SyncVersion.From(1), """{"name":"Recent"}""", DeviceIdentifier.From("device-recent"), DeltaOperation.Create);
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var changes = await store.GetChangedSinceAsync(DateTime.UtcNow.AddMinutes(-5), null, 100);

            changes.Should().Contain(c => c.EntityId == entityId);
        }
    }

    [Fact]
    public async Task GetChangedSince_ShouldReturnCorrectOperation()
    {
        var createEntityId = Guid.NewGuid();
        var updateEntityId = Guid.NewGuid();

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, createEntityId, SyncVersion.From(1), """{"op":"create"}""", DeviceIdentifier.From("device-001"), DeltaOperation.Create);
            await store.SetVersionAsync(EntityType.Project, updateEntityId, SyncVersion.From(2), """{"op":"update"}""", DeviceIdentifier.From("device-001"), DeltaOperation.Update);
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var changes = await store.GetChangedSinceAsync(DateTime.UtcNow.AddMinutes(-5), null, 100);

            var createChange = changes.First(c => c.EntityId == createEntityId);
            createChange.Operation.Should().Be("create");

            var updateChange = changes.First(c => c.EntityId == updateEntityId);
            updateChange.Operation.Should().Be("update");
        }
    }
}
