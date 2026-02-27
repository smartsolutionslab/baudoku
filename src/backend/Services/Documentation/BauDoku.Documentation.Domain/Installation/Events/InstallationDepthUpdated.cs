using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDepthUpdated(
    Guid InstallationId,
    int? DepthMm,
    DateTime OccurredOn) : IDomainEvent;
