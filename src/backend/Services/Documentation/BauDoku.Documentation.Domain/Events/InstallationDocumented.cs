using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Events;

public sealed record InstallationDocumented(
    InstallationId InstallationId,
    Guid ProjectId,
    InstallationType Type,
    DateTime OccurredOn) : IDomainEvent;
