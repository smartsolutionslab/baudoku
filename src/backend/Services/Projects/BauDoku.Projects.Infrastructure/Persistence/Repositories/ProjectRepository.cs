using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.BauDoku.Projects.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(ProjectsDbContext context) : IProjectRepository
{
    private readonly DbSet<Project> projects = context.Projects;

    public async Task<Project> GetByIdAsync(ProjectIdentifier id, CancellationToken cancellationToken = default)
    {
        return (await projects
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken))
            .OrNotFound("Projekt", id.Value);
    }

    public async Task<bool> ExistsByNameAsync(ProjectName name, CancellationToken cancellationToken = default)
    {
        return await projects.AnyAsync(p => p.Name == name, cancellationToken);
    }

    public async Task AddAsync(Project aggregate, CancellationToken cancellationToken = default)
    {
        await projects.AddAsync(aggregate, cancellationToken);
    }

    public void Remove(Project project)
    {
        projects.Remove(project);
    }
}
