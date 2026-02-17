using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Infrastructure.Persistence;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Persistence;

public sealed class ProjectsDbContext(DbContextOptions<ProjectsDbContext> options, IDispatcher dispatcher)
    : BaseDbContext(options, dispatcher)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Zone> Zones => Set<Zone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectsDbContext).Assembly);
    }
}
