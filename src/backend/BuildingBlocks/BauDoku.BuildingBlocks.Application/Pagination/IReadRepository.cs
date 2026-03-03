using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.Application.Pagination;

public interface IReadRepository<TDto, in TId, TListDto, in TFilter> : IReadRepository<TDto, TId> where TId : IValueObject
{
    Task<PagedResult<TListDto>> ListAsync(TFilter filter, PaginationParams pagination, CancellationToken cancellationToken = default);
}
