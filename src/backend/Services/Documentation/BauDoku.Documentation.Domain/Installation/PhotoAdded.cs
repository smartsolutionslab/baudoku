using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record PhotoAdded(
    InstallationIdentifier InstallationIdentifier,
    PhotoIdentifier PhotoIdentifier,
    DateTime OccurredOn) : IDomainEvent;
