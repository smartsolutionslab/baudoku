using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.ReadModel;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetInstallationsInBoundingBoxQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInBoundingBoxQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(GetInstallationsInBoundingBoxQuery query, CancellationToken cancellationToken = default)
    {
        var (bounds, page, pageSize, projectId) = query;

        var pagination = new PaginationParams(page, pageSize);
        return await installations.SearchInBoundingBoxAsync(bounds, projectId, pagination, cancellationToken);
    }
}
