using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Services;

public sealed class ProjectCountProvider : IProjectCountProvider
{
    private readonly ProjectsDbContext dbContext;

    public ProjectCountProvider(ProjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<int> GetActiveCountAsync(CancellationToken ct = default)
    {
        return await dbContext.Projects.CountAsync(ct);
    }
}
