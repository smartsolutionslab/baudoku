using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationDeleted(
    InstallationIdentifier InstallationId,
    ProjectIdentifier ProjectId,
    DateTime OccurredOn) : IDomainEvent;
