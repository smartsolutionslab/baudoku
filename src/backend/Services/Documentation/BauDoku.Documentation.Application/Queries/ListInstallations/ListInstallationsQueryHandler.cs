using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries.ListInstallations;

public sealed class ListInstallationsQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<ListInstallationsQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(ListInstallationsQuery query, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type, status, search, page, pageSize) = query;
        var filter = new InstallationListFilter(
            ProjectIdentifier.FromNullable(projectId),
            ZoneIdentifier.FromNullable(zoneId),
            InstallationType.FromNullable(type),
            InstallationStatus.FromNullable(status),
            search);
        var pagination = new PaginationParams(page, pageSize);
        return await installations.ListAsync(filter, pagination, cancellationToken);
    }
}
