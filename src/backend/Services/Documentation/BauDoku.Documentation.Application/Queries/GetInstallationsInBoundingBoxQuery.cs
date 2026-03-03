using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.ReadModel;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record GetInstallationsInBoundingBoxQuery(
    BoundingBox Bounds,
    PageNumber Page,
    PageSize PageSize,
    ProjectIdentifier? ProjectId = null) : IQuery<PagedResult<InstallationListItemDto>>;
