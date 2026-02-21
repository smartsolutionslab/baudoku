using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.Application.Pagination;

public sealed record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult(IReadOnlyList<T> items, int totalCount, PageNumber page, PageSize pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page.Value;
        PageSize = pageSize.Value;
    }
}
