using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.ReadModel;

public interface IProjectReadRepository
{
    Task<PagedResult<ProjectListItemDto>> ListAsync(SearchTerm? search, PaginationParams pagination, CancellationToken cancellationToken = default);
}
