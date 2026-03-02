using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Contracts;

public interface IProjectReadRepository
{
    Task<PagedResult<ProjectListItemDto>> ListAsync(SearchTerm? search, PaginationParams pagination, CancellationToken cancellationToken = default);
}
