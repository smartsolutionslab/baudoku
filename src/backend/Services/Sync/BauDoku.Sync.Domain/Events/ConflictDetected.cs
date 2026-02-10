using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Events;

public sealed record ConflictDetected(
    SyncBatchId BatchId,
    ConflictRecordId ConflictId,
    EntityReference EntityRef,
    DateTime OccurredOn) : IDomainEvent;
