using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInRadius;

public sealed class GetInstallationsInRadiusQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInRadiusQuery, PagedResult<NearbyInstallationDto>>
{
    public async Task<PagedResult<NearbyInstallationDto>> Handle(GetInstallationsInRadiusQuery query, CancellationToken cancellationToken = default)
    {
        var (latitude, longitude, radiusMeters, projectId, page, pageSize) = query;
        return await installations.SearchInRadiusAsync(
            latitude, longitude, radiusMeters, projectId, page, pageSize, cancellationToken);
    }
}
