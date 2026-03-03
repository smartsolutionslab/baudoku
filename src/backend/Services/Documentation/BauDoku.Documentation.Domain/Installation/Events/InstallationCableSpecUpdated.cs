using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationCableSpecUpdated(
    InstallationIdentifier InstallationId,
    CableType? CableType,
    CrossSection? CrossSection,
    CableColor? CableColor,
    ConductorCount? ConductorCount,
    DateTime OccurredOn) : IDomainEvent;
