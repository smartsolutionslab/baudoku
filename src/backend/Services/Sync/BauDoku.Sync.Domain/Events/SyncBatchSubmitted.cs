using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Events;

public sealed record SyncBatchSubmitted(
    SyncBatchId BatchId,
    DeviceId DeviceId,
    int DeltaCount,
    DateTime OccurredOn) : IDomainEvent;
