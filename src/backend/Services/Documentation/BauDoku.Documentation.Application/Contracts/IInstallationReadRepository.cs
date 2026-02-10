using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Contracts;

public interface IInstallationReadRepository
{
    Task<PagedResult<InstallationListItemDto>> ListAsync(
        Guid? projectId,
        Guid? zoneId,
        string? type,
        string? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
