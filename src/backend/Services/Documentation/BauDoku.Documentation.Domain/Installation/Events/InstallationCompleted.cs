using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationCompleted(
    Guid InstallationId,
    DateTime CompletedAt,
    DateTime OccurredOn) : IDomainEvent;
