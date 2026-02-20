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
        var pagination = new PaginationParams(
            query.Page ?? PageNumber.Default,
            query.PageSize ?? PageSize.Default);
        return await installations.SearchInBoundingBoxAsync(query.Bounds, query.ProjectId, pagination, cancellationToken);
    }
}
