using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record SyncBatchSubmitted(
    SyncBatchIdentifier BatchId,
    DeviceIdentifier DeviceId,
    DateTime OccurredOn) : IDomainEvent;
