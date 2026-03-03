using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Queries;

public sealed record GetInstallationsInBoundingBoxQuery(
    BoundingBox Bounds,
    PageNumber Page,
    PageSize PageSize,
    ProjectIdentifier? ProjectId = null) : IQuery<PagedResult<InstallationListItemDto>>;
