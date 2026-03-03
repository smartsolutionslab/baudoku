using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Projects.ReadModel;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Queries;

public sealed record ListProjectsQuery(SearchTerm? Search, PageNumber Page, PageSize PageSize) : IQuery<PagedResult<ProjectListItemDto>>;
