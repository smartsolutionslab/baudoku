using BauDoku.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.BuildingBlocks.Infrastructure.Pagination;

public static class QueryablePaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, pagination.Page, pagination.PageSize);
    }
}
