using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record ConflictDetected(
    SyncBatchIdentifier BatchId,
    ConflictRecordIdentifier ConflictId,
    EntityReference EntityRef,
    DateTime OccurredOn) : IDomainEvent;
