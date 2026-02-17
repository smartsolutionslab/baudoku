namespace BauDoku.BuildingBlocks.Application.Pagination;

public sealed record PaginationParams(int Page = 1, int PageSize = 20);
