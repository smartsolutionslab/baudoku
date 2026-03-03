using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationDepthUpdated(
    InstallationIdentifier InstallationId,
    int? DepthMm,
    DateTime OccurredOn) : IDomainEvent;
