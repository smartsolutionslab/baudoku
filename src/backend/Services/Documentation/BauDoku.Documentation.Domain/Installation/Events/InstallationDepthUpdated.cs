using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationDepthUpdated(
    InstallationIdentifier InstallationId,
    Depth? Depth,
    DateTime OccurredOn) : IDomainEvent;
