using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain;

public sealed record ConflictResolved(
    ConflictRecordIdentifier ConflictId,
    ConflictResolutionStrategy Strategy,
    DateTime OccurredOn) : IDomainEvent;
