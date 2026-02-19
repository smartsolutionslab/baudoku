using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain;

public sealed record SyncBatchSubmitted(
    SyncBatchIdentifier BatchId,
    DeviceIdentifier DeviceId,
    DateTime OccurredOn) : IDomainEvent;
