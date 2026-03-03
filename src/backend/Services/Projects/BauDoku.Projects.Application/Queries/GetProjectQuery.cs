using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Projects.ReadModel;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Queries;

public sealed record GetProjectQuery(ProjectIdentifier ProjectId) : IQuery<ProjectDto>;
