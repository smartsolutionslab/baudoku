using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.ListInstallations;

public sealed class ListInstallationsQueryHandler
    : IQueryHandler<ListInstallationsQuery, PagedResult<InstallationListItemDto>>
{
    private readonly IInstallationReadRepository readRepository;

    public ListInstallationsQueryHandler(IInstallationReadRepository readRepository)
    {
        this.readRepository = readRepository;
    }

    public async Task<PagedResult<InstallationListItemDto>> Handle(
        ListInstallationsQuery query, CancellationToken cancellationToken)
    {
        return await readRepository.ListAsync(
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
