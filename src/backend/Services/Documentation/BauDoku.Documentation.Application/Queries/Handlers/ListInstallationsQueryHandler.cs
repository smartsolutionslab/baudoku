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
        var filter = new InstallationListFilter(
            query.ProjectId,
            query.ZoneId,
            query.Type,
            query.Status,
            query.Search?.Value);
        var pagination = new PaginationParams(
            query.Page ?? PageNumber.Default,
            query.PageSize ?? PageSize.Default);
        return await installations.ListAsync(filter, pagination, cancellationToken);
    }
}
