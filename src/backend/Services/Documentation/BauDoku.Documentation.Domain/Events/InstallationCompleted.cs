using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record InstallationCompleted(
    InstallationId InstallationId,
    DateTime OccurredOn) : IDomainEvent;
