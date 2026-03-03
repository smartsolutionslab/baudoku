using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record ConflictResolved(
    ConflictRecordIdentifier ConflictId,
    ConflictResolutionStrategy Strategy,
    DateTime OccurredOn) : IDomainEvent;
