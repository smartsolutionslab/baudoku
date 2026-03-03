using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationCompleted(
    InstallationIdentifier InstallationId,
    DateTime CompletedAt,
    DateTime OccurredOn) : IDomainEvent;
