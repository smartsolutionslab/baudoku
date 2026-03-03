using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.ReadModel;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class ListInstallationsQueryHandler(IInstallationReadRepository installations): IQueryHandler<ListInstallationsQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(ListInstallationsQuery query, CancellationToken cancellationToken = default)
    {
        var (page, pageSize, projectId, zoneId, type, status, search) = query;

        var filter = new InstallationListFilter(projectId, zoneId, type, status, search);
        var pagination = new PaginationParams(page, pageSize);
        return await installations.ListAsync(filter, pagination, cancellationToken);
    }
}
