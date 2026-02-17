using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Persistence.Repositories;

public sealed class ProjectReadRepository(ProjectsDbContext context) : IProjectReadRepository
{
    public async Task<PagedResult<ProjectListItemDto>> ListAsync(string? search, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var (page, pageSize) = pagination;

        var query = context.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => EF.Functions.ILike(p.Name.Value, $"%{search}%")
                || EF.Functions.ILike(p.Address.City, $"%{search}%")
                || EF.Functions.ILike(p.Client.Name, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectListItemDto(
                p.Id.Value,
                p.Name.Value,
                p.Status.Value,
                p.Address.City,
                p.Client.Name,
                p.CreatedAt,
                p.Zones.Count))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProjectListItemDto>(items, totalCount, page, pageSize);
    }
}
