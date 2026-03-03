using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public sealed record ProjectCreated(
    ProjectIdentifier ProjectIdentifier,
    ProjectName Name,
    DateTime OccurredOn) : IDomainEvent;
