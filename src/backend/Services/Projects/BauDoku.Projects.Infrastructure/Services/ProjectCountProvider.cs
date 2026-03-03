using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Domain;
using BauDoku.Projects.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Services;

public sealed class ProjectCountProvider(ProjectsDbContext dbContext) : IProjectCountProvider
{
    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Projects.Where(p => p.Status == ProjectStatus.Active).CountAsync(cancellationToken);
    }
}
