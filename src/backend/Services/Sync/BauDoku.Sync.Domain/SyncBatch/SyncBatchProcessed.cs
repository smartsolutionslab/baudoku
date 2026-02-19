using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Events;

public sealed record SyncBatchProcessed(
    SyncBatchIdentifier BatchId,
    int AppliedCount,
    int ConflictCount,
    DateTime OccurredOn) : IDomainEvent;
