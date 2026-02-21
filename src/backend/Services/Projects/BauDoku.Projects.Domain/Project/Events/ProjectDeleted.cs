using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain;

public sealed record ProjectDeleted(
    ProjectIdentifier ProjectIdentifier,
    DateTime OccurredOn) : IDomainEvent;
