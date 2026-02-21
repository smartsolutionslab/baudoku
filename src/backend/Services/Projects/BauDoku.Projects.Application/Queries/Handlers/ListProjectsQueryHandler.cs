using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Queries.Handlers;

public sealed class ListProjectsQueryHandler(IProjectReadRepository projects) : IQueryHandler<ListProjectsQuery, PagedResult<ProjectListItemDto>>
{
    public async Task<PagedResult<ProjectListItemDto>> Handle(ListProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParams(query.Page, query.PageSize);
        return await projects.ListAsync(query.Search?.Value, pagination, cancellationToken);
    }
}
