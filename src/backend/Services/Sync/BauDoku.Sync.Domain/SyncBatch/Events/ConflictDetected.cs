using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain;

public sealed record ConflictDetected(
    SyncBatchIdentifier BatchId,
    ConflictRecordIdentifier ConflictId,
    EntityReference EntityRef,
    DateTime OccurredOn) : IDomainEvent;
