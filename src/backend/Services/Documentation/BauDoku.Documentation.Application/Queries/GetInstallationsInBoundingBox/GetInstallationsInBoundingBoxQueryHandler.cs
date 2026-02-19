using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInBoundingBox;

public sealed class GetInstallationsInBoundingBoxQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<GetInstallationsInBoundingBoxQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(GetInstallationsInBoundingBoxQuery query, CancellationToken cancellationToken = default)
    {
        var (minLatitude, minLongitude, maxLatitude, maxLongitude, projectId, page, pageSize) = query;
        var boundingBox = new BoundingBox(minLatitude, minLongitude, maxLatitude, maxLongitude);
        var projectIdentifier = ProjectIdentifier.FromNullable(projectId);
        var pagination = new PaginationParams(page, pageSize);
        return await installations.SearchInBoundingBoxAsync(boundingBox, projectIdentifier, pagination, cancellationToken);
    }
}
