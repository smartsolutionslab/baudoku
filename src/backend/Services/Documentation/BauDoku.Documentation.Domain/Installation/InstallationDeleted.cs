using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record InstallationDeleted(
    InstallationIdentifier InstallationIdentifier,
    ProjectIdentifier ProjectIdentifier,
    DateTime OccurredOn) : IDomainEvent;
