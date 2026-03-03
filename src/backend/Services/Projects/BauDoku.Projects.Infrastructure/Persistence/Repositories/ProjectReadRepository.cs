using System.Linq.Expressions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Projects.ReadModel;
using BauDoku.Projects.Application.Mapping;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Persistence.Pagination;
using BauDoku.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Persistence.Repositories;

public sealed class ProjectReadRepository(ProjectsDbContext context) : IProjectReadRepository
{
    public async Task<ProjectDto> GetByIdAsync(ProjectIdentifier id, CancellationToken cancellationToken = default)
    {
        var project = (await context.Projects
            .AsNoTracking()
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken))
            .OrNotFound("Projekt", id.Value);

        return project.ToDto();
    }

    private static readonly Expression<Func<Project, ProjectListItemDto>> toProjectListItem = p => new ProjectListItemDto(
        p.Id.Value,
        p.Name.Value,
        p.Status.Value,
        p.Address.City.Value,
        p.Client.Name.Value,
        p.CreatedAt,
        p.Zones.Count);

    public async Task<PagedResult<ProjectListItemDto>> ListAsync(SearchTerm? search, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = context.Projects.AsNoTracking();

        if (search is not null)
        {
            var term = search.Value;
            query = query.Where(p => EF.Functions.ILike(p.Name.Value, $"%{term}%")
                || EF.Functions.ILike(p.Address.City.Value, $"%{term}%")
                || EF.Functions.ILike(p.Client.Name.Value, $"%{term}%"));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(toProjectListItem)
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
