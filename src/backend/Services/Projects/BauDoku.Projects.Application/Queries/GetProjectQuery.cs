using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.ReadModel;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Queries;

public sealed record GetProjectQuery(ProjectIdentifier ProjectId) : IQuery<ProjectDto>;
