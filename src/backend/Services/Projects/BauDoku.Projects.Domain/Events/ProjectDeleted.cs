using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Events;

public sealed record ProjectDeleted(
    ProjectIdentifier ProjectIdentifier,
    DateTime OccurredOn) : IDomainEvent;
