using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class ListInstallationsQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<ListInstallationsQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(ListInstallationsQuery query, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type, status, search, page, pageSize) = query;

        var filter = new InstallationListFilter(
            projectId,
            zoneId,
            type,
            status,
            search?.Value);
        var pagination = new PaginationParams(
            page ?? PageNumber.Default,
            pageSize ?? PageSize.Default);
        return await installations.ListAsync(filter, pagination, cancellationToken);
    }
}
