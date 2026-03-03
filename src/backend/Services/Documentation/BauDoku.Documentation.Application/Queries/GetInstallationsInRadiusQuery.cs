using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.ReadModel;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record GetInstallationsInRadiusQuery(
    SearchRadius Radius,
    ProjectIdentifier? ProjectId = null,
    PageNumber? Page = null,
    PageSize? PageSize = null) : IQuery<PagedResult<NearbyInstallationDto>>;
