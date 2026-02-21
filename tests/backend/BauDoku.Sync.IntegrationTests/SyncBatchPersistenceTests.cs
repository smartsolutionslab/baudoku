using AwesomeAssertions;
using BauDoku.Sync.Domain;
using BauDoku.Sync.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class SyncBatchPersistenceTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task CreateBatch_ShouldPersistAndLoad()
    {
        var batchId = SyncBatchIdentifier.New();
        var batch = SyncBatch.Create(batchId, DeviceIdentifier.From("device-persist"), DateTime.UtcNow);

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Deltas)
                .Include(b => b.Conflicts)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            loaded.Should().NotBeNull();
            loaded.DeviceId.Value.Should().Be("device-persist");
            loaded.Status.Should().Be(BatchStatus.Pending);
            loaded.Deltas.Should().BeEmpty();
            loaded.Conflicts.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task AddDelta_ShouldPersistWithBatch()
    {
        var batchId = SyncBatchIdentifier.New();
        var batch = SyncBatch.Create(batchId, DeviceIdentifier.From("device-delta"), DateTime.UtcNow);

        var entityRef = EntityReference.Create(EntityType.Project, Guid.NewGuid());
        batch.AddDelta(
            SyncDeltaIdentifier.New(),
            entityRef,
            DeltaOperation.Create,
            SyncVersion.Initial,
            SyncVersion.From(1),
            DeltaPayload.From("""{"name":"Test"}"""),
            DateTime.UtcNow);

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Deltas)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            loaded.Should().NotBeNull();
            loaded!.Deltas.Should().ContainSingle();
            loaded.Deltas[0].Payload.Value.Should().Be("""{"name":"Test"}""");
            loaded.Deltas[0].Operation.Should().Be(DeltaOperation.Create);
        }
    }

    [Fact]
    public async Task AddConflict_ShouldPersistWithBatch()
    {
        var batchId = SyncBatchIdentifier.New();
        var batch = SyncBatch.Create(batchId, DeviceIdentifier.From("device-conflict"), DateTime.UtcNow);

        var entityRef = EntityReference.Create(EntityType.Installation, Guid.NewGuid());
        batch.AddConflict(
            ConflictRecordIdentifier.New(),
            entityRef,
            DeltaPayload.From("""{"client":"v1"}"""),
            DeltaPayload.From("""{"server":"v2"}"""),
            SyncVersion.From(1),
            SyncVersion.From(3));

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Conflicts)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            loaded.Should().NotBeNull();
            loaded.Conflicts.Should().ContainSingle();
            loaded.Conflicts[0].ClientPayload.Value.Should().Be("""{"client":"v1"}""");
            loaded.Conflicts[0].ServerPayload.Value.Should().Be("""{"server":"v2"}""");
            loaded.Conflicts[0].Status.Should().Be(ConflictStatus.Unresolved);
        }
    }

    [Fact]
    public async Task GetByConflictId_ShouldReturnBatchContainingConflict()
    {
        var batchId = SyncBatchIdentifier.New();
        var batch = SyncBatch.Create(batchId, DeviceIdentifier.From("device-getby"), DateTime.UtcNow);
        var conflictId = ConflictRecordIdentifier.New();
        var entityRef = EntityReference.Create(EntityType.Project, Guid.NewGuid());

        batch.AddConflict(
            conflictId,
            entityRef,
            DeltaPayload.From("""{"c":"1"}"""),
            DeltaPayload.From("""{"s":"2"}"""),
            SyncVersion.From(0),
            SyncVersion.From(1));

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Deltas)
                .Include(b => b.Conflicts)
                .FirstOrDefaultAsync(b => b.Conflicts.Any(c => c.Id == conflictId));

            loaded.Should().NotBeNull();
            loaded.Id.Should().Be(batchId);
            loaded.Conflicts.Should().Contain(c => c.Id == conflictId);
        }
    }
}
