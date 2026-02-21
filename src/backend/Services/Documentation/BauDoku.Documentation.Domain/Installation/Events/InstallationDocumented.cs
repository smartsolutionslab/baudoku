using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDocumented(
    InstallationIdentifier InstallationIdentifier,
    ProjectIdentifier ProjectIdentifier,
    InstallationType Type,
    DateTime OccurredOn) : IDomainEvent;
