using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Projects.ReadModel;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Queries.Handlers;

public sealed class ListProjectsQueryHandler(IProjectReadRepository projects) : IQueryHandler<ListProjectsQuery, PagedResult<ProjectListItemDto>>
{
    public async Task<PagedResult<ProjectListItemDto>> Handle(ListProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var (search, page, pageSize) = query;

        var pagination = new PaginationParams(page, pageSize);
        return await projects.With(search, pagination, cancellationToken);
    }
}
