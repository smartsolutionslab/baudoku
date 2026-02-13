using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.Events;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.UnitTests.Builders;

namespace BauDoku.Sync.UnitTests.Domain.Aggregates;

public sealed class SyncBatchTests
{
    private static SyncBatch CreateValidBatch() => new SyncBatchBuilder().Build();

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var id = SyncBatchId.New();
        var deviceId = new DeviceId("device-test");
        var now = DateTime.UtcNow;

        var batch = SyncBatch.Create(id, deviceId, now);

        batch.Id.Should().Be(id);
        batch.DeviceId.Should().Be(deviceId);
        batch.Status.Should().Be(BatchStatus.Pending);
        batch.SubmittedAt.Should().Be(now);
        batch.Deltas.Should().BeEmpty();
        batch.Conflicts.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldRaiseSyncBatchSubmittedEvent()
    {
        var batch = CreateValidBatch();

        batch.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SyncBatchSubmitted>();
    }

    [Fact]
    public void AddDelta_ShouldAddDeltaToBatch()
    {
        var batch = CreateValidBatch();
        var entityRef = new EntityReference(EntityType.Project, Guid.NewGuid());

        batch.AddDelta(
            SyncDeltaId.New(),
            entityRef,
            DeltaOperation.Create,
            SyncVersion.Initial,
            new SyncVersion(1),
            new DeltaPayload("""{"name":"Test"}"""),
            DateTime.UtcNow);

        batch.Deltas.Should().ContainSingle();
    }

    [Fact]
    public void AddConflict_ShouldAddConflictToBatch()
    {
        var batch = CreateValidBatch();
        var entityRef = new EntityReference(EntityType.Installation, Guid.NewGuid());

        var conflict = batch.AddConflict(
            ConflictRecordId.New(),
            entityRef,
            new DeltaPayload("""{"client":"data"}"""),
            new DeltaPayload("""{"server":"data"}"""),
            new SyncVersion(1),
            new SyncVersion(2));

        batch.Conflicts.Should().ContainSingle();
        conflict.Status.Should().Be(ConflictStatus.Unresolved);
    }

    [Fact]
    public void AddConflict_ShouldRaiseConflictDetectedEvent()
    {
        var batch = CreateValidBatch();
        batch.ClearDomainEvents();

        batch.AddConflict(
            ConflictRecordId.New(),
            new EntityReference(EntityType.Project, Guid.NewGuid()),
            new DeltaPayload("""{"a":1}"""),
            new DeltaPayload("""{"a":2}"""),
            new SyncVersion(1),
            new SyncVersion(2));

        batch.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ConflictDetected>();
    }

    [Fact]
    public void MarkCompleted_ShouldSetStatusToCompleted()
    {
        var batch = CreateValidBatch();

        batch.MarkCompleted();

        batch.Status.Should().Be(BatchStatus.Completed);
        batch.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkCompleted_ShouldRaiseSyncBatchProcessedEvent()
    {
        var batch = CreateValidBatch();
        batch.ClearDomainEvents();

        batch.MarkCompleted();

        batch.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SyncBatchProcessed>();
    }

    [Fact]
    public void MarkPartialConflict_ShouldSetStatusToPartialConflict()
    {
        var batch = CreateValidBatch();

        batch.MarkPartialConflict();

        batch.Status.Should().Be(BatchStatus.PartialConflict);
    }

    [Fact]
    public void MarkFailed_ShouldSetStatusToFailed()
    {
        var batch = CreateValidBatch();

        batch.MarkFailed();

        batch.Status.Should().Be(BatchStatus.Failed);
    }

    [Fact]
    public void AddDelta_OnCompletedBatch_ShouldThrowBusinessRuleException()
    {
        var batch = CreateValidBatch();
        batch.MarkCompleted();

        var act = () => batch.AddDelta(
            SyncDeltaId.New(),
            new EntityReference(EntityType.Project, Guid.NewGuid()),
            DeltaOperation.Create,
            SyncVersion.Initial,
            new SyncVersion(1),
            new DeltaPayload("""{"x":1}"""),
            DateTime.UtcNow);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void MarkCompleted_OnAlreadyCompletedBatch_ShouldThrowBusinessRuleException()
    {
        var batch = CreateValidBatch();
        batch.MarkCompleted();

        var act = () => batch.MarkCompleted();

        act.Should().Throw<BusinessRuleException>();
    }
}
