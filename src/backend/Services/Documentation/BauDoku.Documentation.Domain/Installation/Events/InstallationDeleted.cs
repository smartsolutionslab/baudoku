using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDeleted(
    InstallationIdentifier InstallationIdentifier,
    ProjectIdentifier ProjectIdentifier,
    DateTime OccurredOn) : IDomainEvent;
