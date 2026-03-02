using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDescriptionUpdated(
    InstallationIdentifier InstallationId,
    Description? Description,
    DateTime OccurredOn) : IDomainEvent;
