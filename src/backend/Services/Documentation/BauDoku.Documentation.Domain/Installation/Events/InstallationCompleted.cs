using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationCompleted(
    InstallationIdentifier InstallationId,
    DateTime CompletedAt,
    DateTime OccurredOn) : IDomainEvent;
