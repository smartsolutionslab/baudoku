using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.Entities;
using BauDoku.Sync.Domain.Events;
using BauDoku.Sync.Domain.Rules;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Aggregates;

public sealed class SyncBatch : AggregateRoot<SyncBatchId>
{
    private readonly List<SyncDelta> _deltas = [];
    private readonly List<ConflictRecord> _conflicts = [];

    public DeviceId DeviceId { get; private set; } = default!;
    public BatchStatus Status { get; private set; } = default!;
    public DateTime SubmittedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public IReadOnlyList<SyncDelta> Deltas => _deltas.AsReadOnly();
    public IReadOnlyList<ConflictRecord> Conflicts => _conflicts.AsReadOnly();

    private SyncBatch() { }

    public static SyncBatch Create(SyncBatchId id, DeviceId deviceId, DateTime submittedAt)
    {
        var batch = new SyncBatch
        {
            Id = id,
            DeviceId = deviceId,
            Status = BatchStatus.Pending,
            SubmittedAt = submittedAt
        };

        batch.AddDomainEvent(new SyncBatchSubmitted(id, deviceId, 0, DateTime.UtcNow));
        return batch;
    }

    public void AddDelta(
        SyncDeltaId deltaId,
        EntityReference entityRef,
        DeltaOperation operation,
        SyncVersion baseVersion,
        SyncVersion serverVersion,
        DeltaPayload payload,
        DateTime timestamp)
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        var delta = SyncDelta.Create(deltaId, entityRef, operation, baseVersion, serverVersion, payload, timestamp);
        _deltas.Add(delta);
    }

    public ConflictRecord AddConflict(
        ConflictRecordId conflictId,
        EntityReference entityRef,
        DeltaPayload clientPayload,
        DeltaPayload serverPayload,
        SyncVersion clientVersion,
        SyncVersion serverVersion)
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        var conflict = ConflictRecord.Create(conflictId, entityRef, clientPayload, serverPayload, clientVersion, serverVersion);
        _conflicts.Add(conflict);

        AddDomainEvent(new ConflictDetected(Id, conflictId, entityRef, DateTime.UtcNow));
        return conflict;
    }

    public void MarkCompleted()
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        Status = BatchStatus.Completed;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new SyncBatchProcessed(Id, _deltas.Count, _conflicts.Count, DateTime.UtcNow));
    }

    public void MarkPartialConflict()
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        Status = BatchStatus.PartialConflict;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new SyncBatchProcessed(Id, _deltas.Count, _conflicts.Count, DateTime.UtcNow));
    }

    public void MarkFailed()
    {
        CheckRule(new BatchMustNotBeAlreadyProcessed(Status));

        Status = BatchStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
    }
}
