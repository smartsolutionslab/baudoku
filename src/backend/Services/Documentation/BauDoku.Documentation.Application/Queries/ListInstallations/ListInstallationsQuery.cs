using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.ListInstallations;

public sealed record ListInstallationsQuery(
    Guid? ProjectId = null,
    Guid? ZoneId = null,
    string? Type = null,
    string? Status = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<InstallationListItemDto>>;
