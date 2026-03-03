using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;

public sealed record PaginationParams(PageNumber Page, PageSize PageSize)
{
    public static PaginationParams Default => new(PageNumber.Default, PageSize.Default);
}
