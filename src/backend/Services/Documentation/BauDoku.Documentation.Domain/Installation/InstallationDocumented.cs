using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record InstallationDocumented(
    InstallationIdentifier InstallationIdentifier,
    ProjectIdentifier ProjectIdentifier,
    InstallationType Type,
    DateTime OccurredOn) : IDomainEvent;
