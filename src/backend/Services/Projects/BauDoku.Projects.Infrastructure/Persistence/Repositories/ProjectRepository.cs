using BauDoku.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(ProjectsDbContext context) : IProjectRepository
{
    public async Task<Project> GetByIdAsync(ProjectIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Projects
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Projekt mit ID '{id.Value}' wurde nicht gefunden.");
    }

    public async Task<Project> GetByIdReadOnlyAsync(ProjectIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Projects
            .AsNoTracking()
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Projekt mit ID '{id.Value}' wurde nicht gefunden.");
    }

    public async Task<bool> ExistsByNameAsync(ProjectName name, CancellationToken ct = default)
    {
        return await context.Projects.AnyAsync(p => p.Name == name, ct);
    }

    public async Task AddAsync(Project aggregate, CancellationToken cancellationToken = default)
    {
        await context.Projects.AddAsync(aggregate, cancellationToken);
    }

    public void Remove(Project project)
    {
        context.Projects.Remove(project);
    }
}
