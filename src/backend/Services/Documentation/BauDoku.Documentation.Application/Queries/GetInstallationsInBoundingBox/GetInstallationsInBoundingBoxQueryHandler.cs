using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInBoundingBox;

public sealed class GetInstallationsInBoundingBoxQueryHandler
    : IQueryHandler<GetInstallationsInBoundingBoxQuery, PagedResult<InstallationListItemDto>>
{
    private readonly IInstallationReadRepository installations;

    public GetInstallationsInBoundingBoxQueryHandler(IInstallationReadRepository installations)
    {
        this.installations = installations;
    }

    public async Task<PagedResult<InstallationListItemDto>> Handle(
        GetInstallationsInBoundingBoxQuery query, CancellationToken cancellationToken)
    {
        return await installations.SearchInBoundingBoxAsync(
            query.MinLatitude,
            query.MinLongitude,
            query.MaxLatitude,
            query.MaxLongitude,
            query.ProjectId,
            query.Page,
            query.PageSize,
            cancellationToken);
    }
}
