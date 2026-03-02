using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDepthUpdated(
    InstallationIdentifier InstallationId,
    int? DepthMm,
    DateTime OccurredOn) : IDomainEvent;
