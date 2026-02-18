using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.ListInstallations;

public sealed class ListInstallationsQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<ListInstallationsQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(ListInstallationsQuery query, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type, status, search, page, pageSize) = query;
        var filter = new InstallationListFilter(
            projectId.HasValue ? ProjectIdentifier.From(projectId.Value) : null,
            zoneId.HasValue ? ZoneIdentifier.From(zoneId.Value) : null,
            type is not null ? InstallationType.From(type) : null,
            status is not null ? InstallationStatus.From(status) : null,
            search);
        var pagination = new PaginationParams(page, pageSize);
        return await installations.ListAsync(filter, pagination, cancellationToken);
    }
}
