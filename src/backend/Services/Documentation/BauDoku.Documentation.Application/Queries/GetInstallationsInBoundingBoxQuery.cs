using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record GetInstallationsInBoundingBoxQuery(
    BoundingBox Bounds,
    ProjectIdentifier? ProjectId = null,
    PageNumber? Page = null,
    PageSize? PageSize = null) : IQuery<PagedResult<InstallationListItemDto>>;
