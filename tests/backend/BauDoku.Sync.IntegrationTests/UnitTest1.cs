using AwesomeAssertions;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.Entities;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.Infrastructure.Persistence;

namespace BauDoku.Sync.IntegrationTests;

/// <summary>
/// Smoke tests verifying domain and infrastructure types resolve correctly.
/// Full database integration tests require Docker (Testcontainers).
/// </summary>
public sealed class SyncInfrastructureSmokeTests
{
    [Fact]
    public void SyncDbContextFactory_ShouldBeInstantiable()
    {
        var factory = new SyncDbContextFactory();
        factory.Should().NotBeNull();
    }

    [Fact]
    public void EntityVersionEntry_ShouldBeInstantiable()
    {
        var entry = new EntityVersionEntry
        {
            EntityType = "project",
            EntityId = Guid.NewGuid(),
            Version = 1,
            Payload = """{"name":"Test"}""",
            LastModified = DateTime.UtcNow,
            LastDeviceId = "device-001"
        };

        entry.EntityType.Should().Be("project");
        entry.Version.Should().Be(1);
    }

    [Fact]
    public void SyncBatch_FullWorkflow_ShouldWorkEndToEnd()
    {
        var batchId = SyncBatchId.New();
        var deviceId = new DeviceId("device-integration");
        var batch = SyncBatch.Create(batchId, deviceId, DateTime.UtcNow);

        var entityRef = new EntityReference(EntityType.Project, Guid.NewGuid());

        batch.AddDelta(
            SyncDeltaId.New(),
            entityRef,
            DeltaOperation.Create,
            SyncVersion.Initial,
            new SyncVersion(1),
            new DeltaPayload("""{"name":"Integration Test"}"""),
            DateTime.UtcNow);

        var conflictRef = new EntityReference(EntityType.Installation, Guid.NewGuid());
        batch.AddConflict(
            ConflictRecordId.New(),
            conflictRef,
            new DeltaPayload("""{"client":"v1"}"""),
            new DeltaPayload("""{"server":"v2"}"""),
            new SyncVersion(1),
            new SyncVersion(3));

        batch.MarkPartialConflict();

        batch.Deltas.Should().HaveCount(1);
        batch.Conflicts.Should().HaveCount(1);
        batch.Status.Should().Be(BatchStatus.PartialConflict);

        var conflict = batch.Conflicts[0];
        conflict.Resolve(ConflictResolutionStrategy.ClientWins);
        conflict.Status.Should().Be(ConflictStatus.ClientWins);
        conflict.ResolvedPayload.Should().NotBeNull();
    }
}
