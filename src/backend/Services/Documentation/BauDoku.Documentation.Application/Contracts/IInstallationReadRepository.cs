using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Contracts;

public interface IInstallationReadRepository
{
    Task<InstallationDto> GetByIdAsync(InstallationIdentifier id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MeasurementDto>> GetMeasurementsAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default);

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
