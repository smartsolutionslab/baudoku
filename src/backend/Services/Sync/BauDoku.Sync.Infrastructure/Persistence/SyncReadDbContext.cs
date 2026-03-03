using SmartSolutionsLab.BauDoku.Sync.Domain;
using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.BauDoku.Sync.Infrastructure.Persistence;

public sealed class SyncReadDbContext(DbContextOptions<SyncReadDbContext> options) : DbContext(options)
{
    public DbSet<SyncBatch> SyncBatches => Set<SyncBatch>();
    public DbSet<SyncDelta> SyncDeltas => Set<SyncDelta>();
    public DbSet<ConflictRecord> ConflictRecords => Set<ConflictRecord>();
    public DbSet<EntityVersionEntry> EntityVersionEntries => Set<EntityVersionEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SyncDbContext).Assembly);
    }
}
