using AwesomeAssertions;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class ConflictPersistenceTests
{
    private readonly PostgreSqlFixture _fixture;

    public ConflictPersistenceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    private static SyncBatch CreateBatchWithConflict(out ConflictRecordId conflictId)
    {
        var batch = SyncBatch.Create(SyncBatchId.New(), new DeviceId("device-conflict-test"), DateTime.UtcNow);
        conflictId = ConflictRecordId.New();
        var entityRef = new EntityReference(EntityType.Project, Guid.NewGuid());

        batch.AddConflict(
            conflictId,
            entityRef,
            new DeltaPayload("""{"client":"data"}"""),
            new DeltaPayload("""{"server":"data"}"""),
            new SyncVersion(1),
            new SyncVersion(5));

        return batch;
    }

    [Fact]
    public async Task Conflict_ShouldPersistAllFields()
    {
        var batch = CreateBatchWithConflict(out var conflictId);

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Conflicts)
                .FirstOrDefaultAsync(b => b.Id == batch.Id);

            loaded.Should().NotBeNull();
            var conflict = loaded!.Conflicts.First(c => c.Id == conflictId);
            conflict.ClientPayload.Value.Should().Be("""{"client":"data"}""");
            conflict.ServerPayload.Value.Should().Be("""{"server":"data"}""");
            conflict.ClientVersion.Value.Should().Be(1);
            conflict.ServerVersion.Value.Should().Be(5);
            conflict.Status.Should().Be(ConflictStatus.Unresolved);
            conflict.ResolvedPayload.Should().BeNull();
            conflict.ResolvedAt.Should().BeNull();
        }
    }

    [Fact]
    public async Task ResolveConflict_ShouldPersistResolution()
    {
        var batch = CreateBatchWithConflict(out var conflictId);

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var updateContext = _fixture.CreateContext())
        {
            var loaded = await updateContext.SyncBatches
                .Include(b => b.Conflicts)
                .FirstAsync(b => b.Id == batch.Id);

            var conflict = loaded.Conflicts.First(c => c.Id == conflictId);
            conflict.Resolve(ConflictResolutionStrategy.ClientWins);
            await updateContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Conflicts)
                .FirstAsync(b => b.Id == batch.Id);

            var conflict = loaded.Conflicts.First(c => c.Id == conflictId);
            conflict.Status.Should().Be(ConflictStatus.ClientWins);
            conflict.ResolvedAt.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task FilterByDeviceId_ShouldReturnOnlyMatchingBatches()
    {
        var deviceId = $"device-filter-{Guid.NewGuid():N}";
        var batch = SyncBatch.Create(SyncBatchId.New(), new DeviceId(deviceId), DateTime.UtcNow);

        batch.AddConflict(
            ConflictRecordId.New(),
            new EntityReference(EntityType.Installation, Guid.NewGuid()),
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
            var batches = await readContext.SyncBatches
                .Include(b => b.Conflicts)
                .Where(b => b.DeviceId == new DeviceId(deviceId))
                .ToListAsync();

            batches.Should().ContainSingle();
            batches[0].Conflicts.Should().ContainSingle();
        }
    }

    [Fact]
    public async Task FilterByConflictStatus_ShouldReturnCorrectConflicts()
    {
        var batch = CreateBatchWithConflict(out var conflictId);

        // Add second conflict that will remain unresolved
        var conflictId2 = ConflictRecordId.New();
        batch.AddConflict(
            conflictId2,
            new EntityReference(EntityType.Zone, Guid.NewGuid()),
            new DeltaPayload("""{"c":"x"}"""),
            new DeltaPayload("""{"s":"y"}"""),
            new SyncVersion(0),
            new SyncVersion(2));

        // Resolve the first conflict
        var conflict1 = batch.Conflicts.First(c => c.Id == conflictId);
        conflict1.Resolve(ConflictResolutionStrategy.ServerWins);

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.SyncBatches
                .Include(b => b.Conflicts)
                .FirstAsync(b => b.Id == batch.Id);

            var unresolvedConflicts = loaded.Conflicts.Where(c => c.Status == ConflictStatus.Unresolved).ToList();
            var resolvedConflicts = loaded.Conflicts.Where(c => c.Status == ConflictStatus.ServerWins).ToList();

            unresolvedConflicts.Should().ContainSingle();
            resolvedConflicts.Should().ContainSingle();
        }
    }
}
