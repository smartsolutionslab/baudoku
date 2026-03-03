using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record PhotoRemoved(
    InstallationIdentifier InstallationId,
    PhotoIdentifier PhotoId,
    DateTime OccurredOn) : IDomainEvent;
