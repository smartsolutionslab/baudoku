using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain;

public sealed record SyncBatchProcessed(
    SyncBatchIdentifier BatchId,
    int AppliedCount,
    int ConflictCount,
    DateTime OccurredOn) : IDomainEvent;
