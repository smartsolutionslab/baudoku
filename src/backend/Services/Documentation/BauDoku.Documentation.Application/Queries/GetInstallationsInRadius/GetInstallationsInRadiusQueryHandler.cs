using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInRadius;

public sealed class GetInstallationsInRadiusQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInRadiusQuery, PagedResult<NearbyInstallationDto>>
{
    public async Task<PagedResult<NearbyInstallationDto>> Handle(
        GetInstallationsInRadiusQuery query, CancellationToken cancellationToken)
    {
        return await installations.SearchInRadiusAsync(
            query.Latitude,
            query.Longitude,
            query.RadiusMeters,
            query.ProjectId,
            query.Page,
            query.PageSize,
            cancellationToken);
    }
}
