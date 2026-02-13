using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInBoundingBox;

public sealed class GetInstallationsInBoundingBoxQueryHandler
    : IQueryHandler<GetInstallationsInBoundingBoxQuery, PagedResult<InstallationListItemDto>>
{
    private readonly IInstallationReadRepository readRepository;

    public GetInstallationsInBoundingBoxQueryHandler(IInstallationReadRepository readRepository)
    {
        this.readRepository = readRepository;
    }

    public async Task<PagedResult<InstallationListItemDto>> Handle(
        GetInstallationsInBoundingBoxQuery query, CancellationToken cancellationToken)
    {
        return await readRepository.SearchInBoundingBoxAsync(
            query.MinLatitude,
            query.MinLongitude,
            query.MaxLatitude,
            query.MaxLongitude,
            query.ProjectId,
            query.Page,
            query.PageSize,
            cancellationToken);
    }
}
