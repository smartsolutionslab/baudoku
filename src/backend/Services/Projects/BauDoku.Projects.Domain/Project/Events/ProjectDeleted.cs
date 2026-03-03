using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public sealed record ProjectDeleted(
    ProjectIdentifier ProjectIdentifier,
    DateTime OccurredOn) : IDomainEvent;
