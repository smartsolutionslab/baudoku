using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.ReadModel;

public interface IInstallationReadRepository : IReadRepository<InstallationDto, InstallationIdentifier, InstallationListItemDto, InstallationListFilter>
{
    Task<IReadOnlyList<MeasurementDto>> GetMeasurementsAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default);

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
