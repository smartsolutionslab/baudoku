using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Queries.ListProjects;

public sealed class ListProjectsQueryHandler(IProjectReadRepository projects)
    : IQueryHandler<ListProjectsQuery, PagedResult<ProjectListItemDto>>
{
    public async Task<PagedResult<ProjectListItemDto>> Handle(ListProjectsQuery query, CancellationToken cancellationToken = default)
    {
        return await projects.ListAsync(query.Search, query.Page, query.PageSize, cancellationToken);
    }
}
