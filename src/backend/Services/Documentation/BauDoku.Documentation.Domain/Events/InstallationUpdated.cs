using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record InstallationUpdated(
    InstallationIdentifier InstallationIdentifier,
    DateTime OccurredOn) : IDomainEvent;
