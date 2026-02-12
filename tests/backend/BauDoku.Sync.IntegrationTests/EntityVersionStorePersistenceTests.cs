using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.Infrastructure.Persistence;
using BauDoku.Sync.Infrastructure.Persistence.Repositories;
using BauDoku.Sync.IntegrationTests.Fixtures;

namespace BauDoku.Sync.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class EntityVersionStorePersistenceTests
{
    private readonly PostgreSqlFixture _fixture;

    public EntityVersionStorePersistenceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SetAndGetVersion_ShouldPersistAndRetrieve()
    {
        var entityId = Guid.NewGuid();

        await using (var context = _fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, new SyncVersion(1), """{"name":"Test"}""", new DeviceId("device-001"));
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
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

        await using (var context = _fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, new SyncVersion(1), """{"v":1}""", new DeviceId("device-001"));
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, new SyncVersion(2), """{"v":2}""", new DeviceId("device-002"));
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
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
        await using var context = _fixture.CreateContext();
        var store = new EntityVersionStore(context);

        var version = await store.GetCurrentVersionAsync(EntityType.Project, Guid.NewGuid());

        version.Should().Be(SyncVersion.Initial);
        version.Value.Should().Be(0);
    }

    [Fact]
    public async Task GetCurrentPayload_WhenNotExists_ShouldReturnNull()
    {
        await using var context = _fixture.CreateContext();
        var store = new EntityVersionStore(context);

        var payload = await store.GetCurrentPayloadAsync(EntityType.Project, Guid.NewGuid());

        payload.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentPayload_WhenExists_ShouldReturnPayload()
    {
        var entityId = Guid.NewGuid();

        await using (var context = _fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Installation, entityId, new SyncVersion(1), """{"data":"value"}""", new DeviceId("device-001"));
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
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

        await using (var context = _fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(EntityType.Project, entityId, new SyncVersion(1), """{"name":"Recent"}""", new DeviceId("device-recent"));
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var changes = await store.GetChangedSinceAsync(DateTime.UtcNow.AddMinutes(-5), null, 100);

            changes.Should().Contain(c => c.EntityId == entityId);
        }
    }
}
