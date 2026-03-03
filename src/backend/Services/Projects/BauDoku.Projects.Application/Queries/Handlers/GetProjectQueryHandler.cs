using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Mapping;
using BauDoku.Projects.ReadModel;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Queries.Handlers;

public sealed class GetProjectQueryHandler(IProjectRepository projects) : IQueryHandler<GetProjectQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        var projectId = query.ProjectId;

        var project = await projects.With(projectId, cancellationToken);

        return project.ToDto();
    }
}
