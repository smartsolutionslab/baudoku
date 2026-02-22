using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetInstallationsInRadiusQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInRadiusQuery, PagedResult<NearbyInstallationDto>>
{
    public async Task<PagedResult<NearbyInstallationDto>> Handle(GetInstallationsInRadiusQuery query, CancellationToken cancellationToken = default)
    {
        var (radius, projectId, page, pageSize) = query;

        var pagination = new PaginationParams(
            page ?? PageNumber.Default,
            pageSize ?? PageSize.Default);
        return await installations.SearchInRadiusAsync(radius, projectId, pagination, cancellationToken);
    }
}
