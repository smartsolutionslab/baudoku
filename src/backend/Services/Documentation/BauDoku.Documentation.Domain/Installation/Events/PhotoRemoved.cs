using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoRemoved(
    InstallationIdentifier InstallationIdentifier,
    PhotoIdentifier PhotoIdentifier,
    DateTime OccurredOn) : IDomainEvent;
