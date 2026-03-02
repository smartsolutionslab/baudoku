using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoRemoved(
    InstallationIdentifier InstallationId,
    PhotoIdentifier PhotoId,
    DateTime OccurredOn) : IDomainEvent;
