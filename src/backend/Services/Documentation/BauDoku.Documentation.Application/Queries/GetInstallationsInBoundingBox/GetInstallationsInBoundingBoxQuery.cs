using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInBoundingBox;

public sealed record GetInstallationsInBoundingBoxQuery(
    double MinLatitude,
    double MinLongitude,
    double MaxLatitude,
    double MaxLongitude,
    Guid? ProjectId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<InstallationListItemDto>>;
