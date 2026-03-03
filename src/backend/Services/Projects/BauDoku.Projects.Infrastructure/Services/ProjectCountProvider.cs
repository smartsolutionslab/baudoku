using SmartSolutionsLab.BauDoku.Projects.Application.Contracts;
using SmartSolutionsLab.BauDoku.Projects.Domain;
using SmartSolutionsLab.BauDoku.Projects.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.BauDoku.Projects.Infrastructure.Services;

public sealed class ProjectCountProvider(ProjectsDbContext dbContext) : IProjectCountProvider
{
    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Projects.Where(p => p.Status == ProjectStatus.Active).CountAsync(cancellationToken);
    }
}
