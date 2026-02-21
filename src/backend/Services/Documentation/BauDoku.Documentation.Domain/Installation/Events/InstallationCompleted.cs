using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationCompleted(
    InstallationIdentifier InstallationIdentifier,
    DateTime OccurredOn) : IDomainEvent;
