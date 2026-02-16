using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Infrastructure.Persistence;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence;

public sealed class DocumentationDbContext(
    DbContextOptions<DocumentationDbContext> options,
    IDispatcher dispatcher)
    : BaseDbContext(options, dispatcher)
{
    public DbSet<Installation> Installations => Set<Installation>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Measurement> Measurements => Set<Measurement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentationDbContext).Assembly);
    }
}
