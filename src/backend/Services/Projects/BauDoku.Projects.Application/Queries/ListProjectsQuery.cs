using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.ReadModel;

namespace BauDoku.Projects.Application.Queries;

public sealed record ListProjectsQuery(SearchTerm? Search, PageNumber Page, PageSize PageSize) : IQuery<PagedResult<ProjectListItemDto>>;
