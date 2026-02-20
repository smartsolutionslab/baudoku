using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Mapping;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Queries.Handlers;

public sealed class GetProjectQueryHandler(IProjectRepository projects) : IQueryHandler<GetProjectQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        var project = await projects.GetByIdReadOnlyAsync(query.ProjectId, cancellationToken);

        return project.ToDto();
    }
}
