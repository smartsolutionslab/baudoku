using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;

public static class PagedReadRepositoryExtensions
{
    public static Task<PagedResult<TListDto>> With<TDto, TId, TListDto, TFilter>(
        this IReadRepository<TDto, TId, TListDto, TFilter> repository,
        TFilter filter,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
        where TId : IValueObject
        => repository.ListAsync(filter, pagination, cancellationToken);
}
