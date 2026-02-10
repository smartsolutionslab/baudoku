using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Queries.ListProjects;

public sealed record ListProjectsQuery(string? Search, int Page = 1, int PageSize = 20) : IQuery<PagedResult<ProjectListItemDto>>;
