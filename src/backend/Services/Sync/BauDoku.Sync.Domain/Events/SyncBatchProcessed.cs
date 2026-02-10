using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Events;

public sealed record SyncBatchProcessed(
    SyncBatchId BatchId,
    int AppliedCount,
    int ConflictCount,
    DateTime OccurredOn) : IDomainEvent;
