using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoAdded(
    InstallationIdentifier InstallationIdentifier,
    PhotoIdentifier PhotoIdentifier,
    DateTime OccurredOn) : IDomainEvent;
