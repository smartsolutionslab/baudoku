using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetInstallationsInBoundingBoxQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInBoundingBoxQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(GetInstallationsInBoundingBoxQuery query, CancellationToken cancellationToken = default)
    {
        var (bounds, projectId, page, pageSize) = query;

        var pagination = new PaginationParams(page ?? PageNumber.Default, pageSize ?? PageSize.Default);
        return await installations.SearchInBoundingBoxAsync(bounds, projectId, pagination, cancellationToken);
    }
}
