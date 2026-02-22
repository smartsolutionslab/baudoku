using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Queries.Handlers;

public sealed class ListProjectsQueryHandler(IProjectReadRepository projects) : IQueryHandler<ListProjectsQuery, PagedResult<ProjectListItemDto>>
{
    public async Task<PagedResult<ProjectListItemDto>> Handle(ListProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var (search, page, pageSize) = query;

        var pagination = new PaginationParams(page, pageSize);
        return await projects.ListAsync(search?.Value, pagination, cancellationToken);
    }
}
