using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain;

public sealed record ProjectCreated(
    ProjectIdentifier ProjectIdentifier,
    ProjectName Name,
    DateTime OccurredOn) : IDomainEvent;
