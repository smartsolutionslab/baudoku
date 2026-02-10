using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Infrastructure.Persistence;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence;

public sealed class DocumentationDbContext : BaseDbContext
{
    public DbSet<Installation> Installations => Set<Installation>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Measurement> Measurements => Set<Measurement>();

    public DocumentationDbContext(
        DbContextOptions<DocumentationDbContext> options,
        IDispatcher dispatcher)
        : base(options, dispatcher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentationDbContext).Assembly);
    }
}
