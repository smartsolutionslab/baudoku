using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Mapping;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Queries.GetProject;

public sealed class GetProjectQueryHandler(IProjectRepository projects) : IQueryHandler<GetProjectQuery, ProjectDto?>
{
    public async Task<ProjectDto?> Handle(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        var projectId = ProjectIdentifier.From(query.ProjectId);
        var project = await projects.GetByIdReadOnlyAsync(projectId, cancellationToken);

        return project?.ToDto();
    }
}
