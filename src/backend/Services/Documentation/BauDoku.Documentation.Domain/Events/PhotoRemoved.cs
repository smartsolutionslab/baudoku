using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record PhotoRemoved(
    InstallationId InstallationId,
    PhotoId PhotoId,
    DateTime OccurredOn) : IDomainEvent;
