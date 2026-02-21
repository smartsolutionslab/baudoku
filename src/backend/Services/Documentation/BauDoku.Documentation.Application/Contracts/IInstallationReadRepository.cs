using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Contracts;

public interface IInstallationReadRepository
{
    Task<PagedResult<InstallationListItemDto>> ListAsync(
        InstallationListFilter filter,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);

    Task<PagedResult<NearbyInstallationDto>> SearchInRadiusAsync(
        SearchRadius radius,
        ProjectIdentifier? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InstallationListItemDto>> SearchInBoundingBoxAsync(
        BoundingBox boundingBox,
        ProjectIdentifier? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);
}
