using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoRemoved(
    Guid InstallationId,
    Guid PhotoId,
    DateTime OccurredOn) : IDomainEvent;
