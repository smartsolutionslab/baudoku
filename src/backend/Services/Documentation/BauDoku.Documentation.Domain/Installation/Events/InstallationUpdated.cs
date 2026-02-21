using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationUpdated(
    InstallationIdentifier InstallationIdentifier,
    DateTime OccurredOn) : IDomainEvent;
