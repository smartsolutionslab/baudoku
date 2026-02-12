using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallationsInRadius;

public sealed record GetInstallationsInRadiusQuery(
    double Latitude,
    double Longitude,
    double RadiusMeters,
    Guid? ProjectId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<NearbyInstallationDto>>;
