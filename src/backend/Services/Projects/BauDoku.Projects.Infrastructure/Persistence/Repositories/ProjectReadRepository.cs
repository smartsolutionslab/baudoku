using System.Linq.Expressions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Infrastructure.Pagination;
using BauDoku.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Persistence.Repositories;

public sealed class ProjectReadRepository(ProjectsDbContext context) : IProjectReadRepository
{
    private static readonly Expression<Func<Project, ProjectListItemDto>> toProjectListItem = p => new ProjectListItemDto(
        p.Id.Value,
        p.Name.Value,
        p.Status.Value,
        p.Address.City.Value,
        p.Client.Name.Value,
        p.CreatedAt,
        p.Zones.Count);

    public async Task<PagedResult<ProjectListItemDto>> ListAsync(string? search, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = context.Projects.AsNoTracking();

        if (search.HasValue())
        {
            query = query.Where(p => EF.Functions.ILike(p.Name.Value, $"%{search}%")
                || EF.Functions.ILike(p.Address.City.Value, $"%{search}%")
                || EF.Functions.ILike(p.Client.Name.Value, $"%{search}%"));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(toProjectListItem)
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
