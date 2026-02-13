using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Events;

public sealed record SyncBatchSubmitted(
    SyncBatchIdentifier BatchId,
    DeviceIdentifier DeviceId,
    int DeltaCount,
    DateTime OccurredOn) : IDomainEvent;
