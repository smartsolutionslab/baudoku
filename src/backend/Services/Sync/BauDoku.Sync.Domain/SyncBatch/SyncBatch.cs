using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain;

public sealed class SyncBatch : AggregateRoot<SyncBatchIdentifier>
{
    private readonly List<SyncDelta> deltas = [];
    private readonly List<ConflictRecord> conflicts = [];

    public DeviceIdentifier DeviceId { get; private set; } = default!;
    public BatchStatus Status { get; private set; } = default!;
    public DateTime SubmittedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public IReadOnlyList<SyncDelta> Deltas => deltas.AsReadOnly();
    public IReadOnlyList<ConflictRecord> Conflicts => conflicts.AsReadOnly();

    private SyncBatch() { }

    public static SyncBatch Create(SyncBatchIdentifier id, DeviceIdentifier deviceId, DateTime submittedAt)
    {
        var batch = new SyncBatch
        {
            Id = id,
            DeviceId = deviceId,
            Status = BatchStatus.Pending,
            SubmittedAt = submittedAt
        };

        batch.AddDomainEvent(new SyncBatchSubmitted(id, deviceId, DateTime.UtcNow));
        return batch;
    }

    public void AddDelta(
        SyncDeltaIdentifier deltaId,
        EntityReference entityRef,
        DeltaOperation operation,
        SyncVersion baseVersion,
        SyncVersion serverVersion,
        DeltaPayload payload,
        DateTime timestamp)
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        var delta = SyncDelta.Create(deltaId, entityRef, operation, baseVersion, serverVersion, payload, timestamp);
        deltas.Add(delta);
    }

    public ConflictRecord AddConflict(
        ConflictRecordIdentifier conflictId,
        EntityReference entityRef,
        DeltaPayload clientPayload,
        DeltaPayload serverPayload,
        SyncVersion clientVersion,
        SyncVersion serverVersion)
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        var conflict = ConflictRecord.Create(conflictId, entityRef, clientPayload, serverPayload, clientVersion, serverVersion);
        conflicts.Add(conflict);

        AddDomainEvent(new ConflictDetected(Id, conflictId, entityRef, DateTime.UtcNow));
        return conflict;
    }

    public void MarkCompleted()
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        Status = BatchStatus.Completed;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new SyncBatchProcessed(Id, deltas.Count, conflicts.Count, DateTime.UtcNow));
    }

    public void MarkPartialConflict()
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        Status = BatchStatus.PartialConflict;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new SyncBatchProcessed(Id, deltas.Count, conflicts.Count, DateTime.UtcNow));
    }

    public void ResolveConflict(
        ConflictRecordIdentifier conflictId,
        ConflictResolutionStrategy strategy,
        DeltaPayload? mergedPayload = null)
    {
        var conflict = conflicts.FirstOrDefault(c => c.Id == conflictId)
            ?? throw new InvalidOperationException($"Konflikt {conflictId.Value} nicht gefunden.");

        conflict.Resolve(strategy, mergedPayload);
        AddDomainEvent(new ConflictResolved(conflictId, strategy, DateTime.UtcNow));
    }

    public void MarkFailed()
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        Status = BatchStatus.Failed;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new SyncBatchProcessed(Id, deltas.Count, conflicts.Count, DateTime.UtcNow));
    }
}
