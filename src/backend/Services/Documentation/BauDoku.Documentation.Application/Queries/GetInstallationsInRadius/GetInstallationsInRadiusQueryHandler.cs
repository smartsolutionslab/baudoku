using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInRadius;

public sealed class GetInstallationsInRadiusQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInRadiusQuery, PagedResult<NearbyInstallationDto>>
{
    public async Task<PagedResult<NearbyInstallationDto>> Handle(GetInstallationsInRadiusQuery query, CancellationToken cancellationToken = default)
    {
        var (latitude, longitude, radiusMeters, projectId, page, pageSize) = query;
        var radius = new SearchRadius(latitude, longitude, radiusMeters);
        var projectIdentifier = ProjectIdentifier.FromNullable(projectId);
        var pagination = new PaginationParams(page, pageSize);
        return await installations.SearchInRadiusAsync(radius, projectIdentifier, pagination, cancellationToken);
    }
}
