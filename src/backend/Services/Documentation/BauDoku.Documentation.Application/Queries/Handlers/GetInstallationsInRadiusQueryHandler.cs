using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetInstallationsInRadiusQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInRadiusQuery, PagedResult<NearbyInstallationDto>>
{
    public async Task<PagedResult<NearbyInstallationDto>> Handle(GetInstallationsInRadiusQuery query, CancellationToken cancellationToken = default)
    {
        var (radius, page, pageSize, projectId) = query;

        var pagination = new PaginationParams(page, pageSize);
        return await installations.SearchInRadiusAsync(radius, projectId, pagination, cancellationToken);
    }
}
