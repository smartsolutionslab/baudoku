using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationDeleted(
    Guid InstallationId,
    Guid ProjectId,
    DateTime OccurredOn) : IDomainEvent;
