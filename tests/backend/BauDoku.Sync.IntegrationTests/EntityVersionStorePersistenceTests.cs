using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.Infrastructure.Persistence.Repositories;
using BauDoku.Sync.IntegrationTests.Fixtures;

namespace BauDoku.Sync.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class EntityVersionStorePersistenceTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task SetAndGetVersion_ShouldPersistAndRetrieve()
    {
        var entityId = Guid.NewGuid();
        var entityRef = EntityReference.Create(EntityType.Project, entityId);

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(entityRef, SyncVersion.From(1), """{"name":"Test"}""", DeviceIdentifier.From("device-001"));
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var version = await store.GetCurrentVersionAsync(entityRef);
            version.Value.Should().Be(1);
        }
    }

    [Fact]
    public async Task SetVersion_WhenEntryExists_ShouldUpdate()
    {
        var entityId = Guid.NewGuid();
        var entityRef = EntityReference.Create(EntityType.Project, entityId);

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(entityRef, SyncVersion.From(1), """{"v":1}""", DeviceIdentifier.From("device-001"));
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(entityRef, SyncVersion.From(2), """{"v":2}""", DeviceIdentifier.From("device-002"));
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var version = await store.GetCurrentVersionAsync(entityRef);
            version.Value.Should().Be(2);

            var payload = await store.GetCurrentPayloadAsync(entityRef);
            payload.Should().Be("""{"v":2}""");
        }
    }

    [Fact]
    public async Task GetCurrentVersion_WhenNotExists_ShouldReturnInitial()
    {
        await using var context = fixture.CreateContext();
        var store = new EntityVersionStore(context);

        var version = await store.GetCurrentVersionAsync(EntityReference.Create(EntityType.Project, Guid.NewGuid()));

        version.Should().Be(SyncVersion.Initial);
        version.Value.Should().Be(0);
    }

    [Fact]
    public async Task GetCurrentPayload_WhenNotExists_ShouldReturnEmptyJson()
    {
        await using var context = fixture.CreateContext();
        var store = new EntityVersionStore(context);

        var payload = await store.GetCurrentPayloadAsync(EntityReference.Create(EntityType.Project, Guid.NewGuid()));

        payload.Should().Be("{}");
    }

    [Fact]
    public async Task GetCurrentPayload_WhenExists_ShouldReturnPayload()
    {
        var entityId = Guid.NewGuid();
        var entityRef = EntityReference.Create(EntityType.Installation, entityId);

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(entityRef, SyncVersion.From(1), """{"data":"value"}""", DeviceIdentifier.From("device-001"));
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var payload = await store.GetCurrentPayloadAsync(entityRef);
            payload.Should().Be("""{"data":"value"}""");
        }
    }

    [Fact]
    public async Task GetChangedSince_ShouldReturnRecentChanges()
    {
        var entityId = Guid.NewGuid();
        var entityRef = EntityReference.Create(EntityType.Project, entityId);

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            await store.SetVersionAsync(entityRef, SyncVersion.From(1), """{"name":"Recent"}""", DeviceIdentifier.From("device-recent"));
            await context.SaveChangesAsync();
        }

        await using (var context = fixture.CreateContext())
        {
            var store = new EntityVersionStore(context);
            var changes = await store.GetChangedSinceAsync(DateTime.UtcNow.AddMinutes(-5), null, 100);

            changes.Should().Contain(c => c.EntityId == entityId);
        }
    }
}
