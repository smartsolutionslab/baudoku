using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Events;

public sealed record ConflictDetected(
    SyncBatchIdentifier BatchId,
    ConflictRecordIdentifier ConflictId,
    EntityReference EntityRef,
    DateTime OccurredOn) : IDomainEvent;
