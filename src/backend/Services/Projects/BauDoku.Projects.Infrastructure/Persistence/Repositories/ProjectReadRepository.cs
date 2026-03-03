using System.Linq.Expressions;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.Projects.ReadModel;
using SmartSolutionsLab.BauDoku.Projects.Application.Mapping;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Persistence.Pagination;
using SmartSolutionsLab.BauDoku.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.BauDoku.Projects.Infrastructure.Persistence.Repositories;

public sealed class ProjectReadRepository(ProjectsReadDbContext context) : IProjectReadRepository
{
    public async Task<ProjectDto> GetByIdAsync(ProjectIdentifier id, CancellationToken cancellationToken = default)
    {
        var project = (await context.Projects
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
        var query = context.Projects.AsQueryable();

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
