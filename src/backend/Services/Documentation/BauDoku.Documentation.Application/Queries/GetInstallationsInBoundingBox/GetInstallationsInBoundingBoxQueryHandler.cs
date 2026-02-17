using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInBoundingBox;

public sealed class GetInstallationsInBoundingBoxQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInBoundingBoxQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(GetInstallationsInBoundingBoxQuery query, CancellationToken cancellationToken = default)
    {
        var (minLatitude, minLongitude, maxLatitude, maxLongitude, projectId, page, pageSize) = query;
        return await installations.SearchInBoundingBoxAsync(
            minLatitude, minLongitude, maxLatitude, maxLongitude, projectId, page, pageSize, cancellationToken);
    }
}
