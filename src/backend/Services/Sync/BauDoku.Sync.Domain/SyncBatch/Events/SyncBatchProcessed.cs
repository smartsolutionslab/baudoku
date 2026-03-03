using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record SyncBatchProcessed(
    SyncBatchIdentifier BatchId,
    int AppliedCount,
    int ConflictCount,
    DateTime OccurredOn) : IDomainEvent;
