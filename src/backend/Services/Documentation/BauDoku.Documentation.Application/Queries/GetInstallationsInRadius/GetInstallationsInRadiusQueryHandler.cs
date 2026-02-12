using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInRadius;

public sealed class GetInstallationsInRadiusQueryHandler
    : IQueryHandler<GetInstallationsInRadiusQuery, PagedResult<NearbyInstallationDto>>
{
    private readonly IInstallationReadRepository _readRepository;

    public GetInstallationsInRadiusQueryHandler(IInstallationReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PagedResult<NearbyInstallationDto>> Handle(
        GetInstallationsInRadiusQuery query, CancellationToken cancellationToken)
    {
        return await _readRepository.SearchInRadiusAsync(
            query.Latitude,
            query.Longitude,
            query.RadiusMeters,
            query.ProjectId,
            query.Page,
            query.PageSize,
            cancellationToken);
    }
}
