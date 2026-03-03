using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationDescriptionUpdated(
    InstallationIdentifier InstallationId,
    Description? Description,
    DateTime OccurredOn) : IDomainEvent;
