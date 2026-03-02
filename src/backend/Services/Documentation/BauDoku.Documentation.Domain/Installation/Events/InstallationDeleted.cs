using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDeleted(
    InstallationIdentifier InstallationId,
    ProjectIdentifier ProjectId,
    DateTime OccurredOn) : IDomainEvent;
