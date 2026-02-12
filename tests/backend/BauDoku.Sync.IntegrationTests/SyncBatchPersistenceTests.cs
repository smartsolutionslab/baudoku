using AwesomeAssertions;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class SyncBatchPersistenceTests
{
    private readonly PostgreSqlFixture _fixture;

    public SyncBatchPersistenceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateBatch_ShouldPersistAndLoad()
    {
        var batchId = SyncBatchId.New();
        var batch = SyncBatch.Create(batchId, new DeviceId("device-persist"), DateTime.UtcNow);

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Deltas)
                .Include(b => b.Conflicts)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            loaded.Should().NotBeNull();
            loaded!.DeviceId.Value.Should().Be("device-persist");
            loaded.Status.Should().Be(BatchStatus.Pending);
            loaded.Deltas.Should().BeEmpty();
            loaded.Conflicts.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task AddDelta_ShouldPersistWithBatch()
    {
        var batchId = SyncBatchId.New();
        var batch = SyncBatch.Create(batchId, new DeviceId("device-delta"), DateTime.UtcNow);

        var entityRef = new EntityReference(EntityType.Project, Guid.NewGuid());
        batch.AddDelta(
            SyncDeltaId.New(),
            entityRef,
            DeltaOperation.Create,
            SyncVersion.Initial,
            new SyncVersion(1),
            new DeltaPayload("""{"name":"Test"}"""),
            DateTime.UtcNow);

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
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
        var batchId = SyncBatchId.New();
        var batch = SyncBatch.Create(batchId, new DeviceId("device-conflict"), DateTime.UtcNow);

        var entityRef = new EntityReference(EntityType.Installation, Guid.NewGuid());
        batch.AddConflict(
            ConflictRecordId.New(),
            entityRef,
            new DeltaPayload("""{"client":"v1"}"""),
            new DeltaPayload("""{"server":"v2"}"""),
            new SyncVersion(1),
            new SyncVersion(3));

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Conflicts)
                .FirstOrDefaultAsync(b => b.Id == batchId);

            loaded.Should().NotBeNull();
            loaded!.Conflicts.Should().ContainSingle();
            loaded.Conflicts[0].ClientPayload.Value.Should().Be("""{"client":"v1"}""");
            loaded.Conflicts[0].ServerPayload.Value.Should().Be("""{"server":"v2"}""");
            loaded.Conflicts[0].Status.Should().Be(ConflictStatus.Unresolved);
        }
    }

    [Fact]
    public async Task GetByConflictId_ShouldReturnBatchContainingConflict()
    {
        var batchId = SyncBatchId.New();
        var batch = SyncBatch.Create(batchId, new DeviceId("device-getby"), DateTime.UtcNow);
        var conflictId = ConflictRecordId.New();
        var entityRef = new EntityReference(EntityType.Project, Guid.NewGuid());

        batch.AddConflict(
            conflictId,
            entityRef,
            new DeltaPayload("""{"c":"1"}"""),
            new DeltaPayload("""{"s":"2"}"""),
            new SyncVersion(0),
            new SyncVersion(1));

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Deltas)
                .Include(b => b.Conflicts)
                .FirstOrDefaultAsync(b => b.Conflicts.Any(c => c.Id == conflictId));

            loaded.Should().NotBeNull();
            loaded!.Id.Should().Be(batchId);
            loaded.Conflicts.Should().Contain(c => c.Id == conflictId);
        }
    }
}
