namespace BauDoku.BuildingBlocks.Application.Pagination;

public static class PagedReadRepositoryExtensions
{
    public static Task<PagedResult<TDto>> With<TDto, TFilter>(
        this IPagedReadRepository<TDto, TFilter> repository,
        TFilter filter,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
        => repository.ListAsync(filter, pagination, cancellationToken);
}
