using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Contracts;

public interface IInstallationReadRepository
{
    Task<PagedResult<InstallationListItemDto>> ListAsync(
        Guid? projectId,
        Guid? zoneId,
        string? type,
        string? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<NearbyInstallationDto>> SearchInRadiusAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        Guid? projectId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<InstallationListItemDto>> SearchInBoundingBoxAsync(
        double minLatitude,
        double minLongitude,
        double maxLatitude,
        double maxLongitude,
        Guid? projectId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
