using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record ListInstallationsQuery(
    ProjectIdentifier? ProjectId = null,
    ZoneIdentifier? ZoneId = null,
    InstallationType? Type = null,
    InstallationStatus? Status = null,
    SearchTerm? Search = null,
    PageNumber? Page = null,
    PageSize? PageSize = null) : IQuery<PagedResult<InstallationListItemDto>>;
