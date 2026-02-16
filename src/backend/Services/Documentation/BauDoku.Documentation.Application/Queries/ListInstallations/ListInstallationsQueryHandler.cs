using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.ListInstallations;

public sealed class ListInstallationsQueryHandler
    : IQueryHandler<ListInstallationsQuery, PagedResult<InstallationListItemDto>>
{
    private readonly IInstallationReadRepository installations;

    public ListInstallationsQueryHandler(IInstallationReadRepository installations)
    {
        this.installations = installations;
    }

    public async Task<PagedResult<InstallationListItemDto>> Handle(
        ListInstallationsQuery query, CancellationToken cancellationToken)
    {
        return await installations.ListAsync(
            query.ProjectId,
            query.ZoneId,
            query.Type,
            query.Status,
            query.Search,
            query.Page,
            query.PageSize,
            cancellationToken);
    }
}
