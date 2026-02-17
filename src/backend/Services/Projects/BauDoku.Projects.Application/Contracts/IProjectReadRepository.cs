using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Contracts;

public interface IProjectReadRepository
{
    Task<PagedResult<ProjectListItemDto>> ListAsync(string? search, PaginationParams pagination, CancellationToken cancellationToken = default);
}
