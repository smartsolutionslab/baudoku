using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.ListInstallations;

public sealed class ListInstallationsQueryHandler(IInstallationReadRepository installations)
    : IQueryHandler<ListInstallationsQuery, PagedResult<InstallationListItemDto>>
{
    public async Task<PagedResult<InstallationListItemDto>> Handle(ListInstallationsQuery query, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type, status, search, page, pageSize) = query;
        return await installations.ListAsync(projectId, zoneId, type, status, search, page, pageSize, cancellationToken);
    }
}
