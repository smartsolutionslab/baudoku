using AwesomeAssertions;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class ConflictPersistenceTests
{
    private readonly PostgreSqlFixture fixture;

    public ConflictPersistenceTests(PostgreSqlFixture fixture)
    {
        this.fixture = fixture;
    }

    private static SyncBatch CreateBatchWithConflict(out ConflictRecordIdentifier conflictId)
    {
        var batch = SyncBatch.Create(SyncBatchIdentifier.New(), DeviceIdentifier.From("device-conflict-test"), DateTime.UtcNow);
        conflictId = ConflictRecordIdentifier.New();
        var entityRef = EntityReference.Create(EntityType.Project, Guid.NewGuid());

        batch.AddConflict(
            conflictId,
            entityRef,
            DeltaPayload.From("""{"client":"data"}"""),
            DeltaPayload.From("""{"server":"data"}"""),
            SyncVersion.From(1),
            SyncVersion.From(5));

        return batch;
    }

    [Fact]
    public async Task Conflict_ShouldPersistAllFields()
    {
        var batch = CreateBatchWithConflict(out var conflictId);

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
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

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var updateContext = fixture.CreateContext())
        {
            var loaded = await updateContext.SyncBatches
                .Include(b => b.Conflicts)
                .FirstAsync(b => b.Id == batch.Id);

            loaded.ResolveConflict(conflictId, ConflictResolutionStrategy.ClientWins);
            await updateContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
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
        var batch = SyncBatch.Create(SyncBatchIdentifier.New(), DeviceIdentifier.From(deviceId), DateTime.UtcNow);

        batch.AddConflict(
            ConflictRecordIdentifier.New(),
            EntityReference.Create(EntityType.Installation, Guid.NewGuid()),
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
            var batches = await readContext.SyncBatches
                .Include(b => b.Conflicts)
                .Where(b => b.DeviceId == DeviceIdentifier.From(deviceId))
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
        var conflictId2 = ConflictRecordIdentifier.New();
        batch.AddConflict(
            conflictId2,
            EntityReference.Create(EntityType.Zone, Guid.NewGuid()),
            DeltaPayload.From("""{"c":"x"}"""),
            DeltaPayload.From("""{"s":"y"}"""),
            SyncVersion.From(0),
            SyncVersion.From(2));

        // Resolve the first conflict
        batch.ResolveConflict(conflictId, ConflictResolutionStrategy.ServerWins);

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.SyncBatches.Add(batch);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
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
