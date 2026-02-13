using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ProjectsDbContext context;

    public ProjectRepository(ProjectsDbContext context)
    {
        this.context = context;
    }

    public async Task<Project?> GetByIdAsync(ProjectIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Projects
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(ProjectName name, CancellationToken ct = default)
    {
        return await context.Projects
            .AnyAsync(p => p.Name == name, ct);
    }

    public async Task AddAsync(Project aggregate, CancellationToken cancellationToken = default)
    {
        await context.Projects.AddAsync(aggregate, cancellationToken);
    }
}
