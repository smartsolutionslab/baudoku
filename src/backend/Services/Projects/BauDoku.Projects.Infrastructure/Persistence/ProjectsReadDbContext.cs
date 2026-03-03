using BauDoku.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Projects.Infrastructure.Persistence;

public sealed class ProjectsReadDbContext(DbContextOptions<ProjectsReadDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Zone> Zones => Set<Zone>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectsDbContext).Assembly);
    }
}
