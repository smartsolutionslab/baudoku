namespace BauDoku.BuildingBlocks.Application.Pagination;

public interface IPagedReadRepository<TDto, in TFilter>
{
    Task<PagedResult<TDto>> ListAsync(TFilter filter, PaginationParams pagination, CancellationToken cancellationToken = default);
}
