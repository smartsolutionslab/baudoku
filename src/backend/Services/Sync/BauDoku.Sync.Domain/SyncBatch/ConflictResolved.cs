using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Domain.Events;

public sealed record ConflictResolved(
    ConflictRecordIdentifier ConflictId,
    ConflictResolutionStrategy Strategy,
    DateTime OccurredOn) : IDomainEvent;
