using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Contracts;

public interface IInstallationReadRepository
{
    Task<PagedResult<InstallationListItemDto>> ListAsync(
        InstallationListFilter filter,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);

    Task<PagedResult<NearbyInstallationDto>> SearchInRadiusAsync(
        SearchRadius radius,
        Guid? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InstallationListItemDto>> SearchInBoundingBoxAsync(
        BoundingBox boundingBox,
        Guid? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);
}
