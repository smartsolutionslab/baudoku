using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Projects.ReadModel;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Queries.Handlers;

public sealed class GetProjectQueryHandler(IProjectReadRepository projects) : IQueryHandler<GetProjectQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        return await projects.With(query.ProjectId, cancellationToken);
    }
}
