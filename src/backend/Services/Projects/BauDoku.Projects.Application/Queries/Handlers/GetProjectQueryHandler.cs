using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.ReadModel;

namespace BauDoku.Projects.Application.Queries.Handlers;

public sealed class GetProjectQueryHandler(IProjectReadRepository projects) : IQueryHandler<GetProjectQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        return await projects.With(query.ProjectId, cancellationToken);
    }
}
