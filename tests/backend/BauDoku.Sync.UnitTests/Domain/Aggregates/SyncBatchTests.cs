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
        var id = SyncBatchIdentifier.New();
        var deviceId = DeviceIdentifier.From("device-test");
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
        var entityRef = EntityReference.Create(EntityType.Project, Guid.NewGuid());

        batch.AddDelta(
            SyncDeltaIdentifier.New(),
            entityRef,
            DeltaOperation.Create,
            SyncVersion.Initial,
            SyncVersion.From(1),
            DeltaPayload.From("""{"name":"Test"}"""),
            DateTime.UtcNow);

        batch.Deltas.Should().ContainSingle();
    }

    [Fact]
    public void AddConflict_ShouldAddConflictToBatch()
    {
        var batch = CreateValidBatch();
        var entityRef = EntityReference.Create(EntityType.Installation, Guid.NewGuid());

        var conflict = batch.AddConflict(
            ConflictRecordIdentifier.New(),
            entityRef,
            DeltaPayload.From("""{"client":"data"}"""),
            DeltaPayload.From("""{"server":"data"}"""),
            SyncVersion.From(1),
            SyncVersion.From(2));

        batch.Conflicts.Should().ContainSingle();
        conflict.Status.Should().Be(ConflictStatus.Unresolved);
    }

    [Fact]
    public void AddConflict_ShouldRaiseConflictDetectedEvent()
    {
        var batch = CreateValidBatch();
        batch.ClearDomainEvents();

        batch.AddConflict(
            ConflictRecordIdentifier.New(),
            EntityReference.Create(EntityType.Project, Guid.NewGuid()),
            DeltaPayload.From("""{"a":1}"""),
            DeltaPayload.From("""{"a":2}"""),
            SyncVersion.From(1),
            SyncVersion.From(2));

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
    public void MarkFailed_ShouldRaiseSyncBatchProcessedEvent()
    {
        var batch = CreateValidBatch();
        batch.ClearDomainEvents();

        batch.MarkFailed();

        batch.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SyncBatchProcessed>();
    }

    [Fact]
    public void ResolveConflict_ShouldResolveAndRaiseConflictResolvedEvent()
    {
        var batch = CreateValidBatch();
        var conflictId = ConflictRecordIdentifier.New();
        batch.AddConflict(
            conflictId,
            EntityReference.Create(EntityType.Project, Guid.NewGuid()),
            DeltaPayload.From("""{"c":1}"""),
            DeltaPayload.From("""{"s":2}"""),
            SyncVersion.From(1),
            SyncVersion.From(2));
        batch.ClearDomainEvents();

        batch.ResolveConflict(conflictId, ConflictResolutionStrategy.ClientWins);

        batch.Conflicts.First(c => c.Id == conflictId).Status
            .Should().Be(ConflictStatus.ClientWins);
        batch.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ConflictResolved>();
    }

    [Fact]
    public void ResolveConflict_WithInvalidId_ShouldThrow()
    {
        var batch = CreateValidBatch();
        var unknownId = ConflictRecordIdentifier.New();

        var act = () => batch.ResolveConflict(unknownId, ConflictResolutionStrategy.ServerWins);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddDelta_OnCompletedBatch_ShouldThrowBusinessRuleException()
    {
        var batch = CreateValidBatch();
        batch.MarkCompleted();

        var act = () => batch.AddDelta(
            SyncDeltaIdentifier.New(),
            EntityReference.Create(EntityType.Project, Guid.NewGuid()),
            DeltaOperation.Create,
            SyncVersion.Initial,
            SyncVersion.From(1),
            DeltaPayload.From("""{"x":1}"""),
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
