using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Queries.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IQuery<ProjectDto?>;
