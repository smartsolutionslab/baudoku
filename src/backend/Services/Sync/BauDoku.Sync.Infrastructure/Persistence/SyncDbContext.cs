using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Infrastructure.Persistence;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence;

public sealed class SyncDbContext : BaseDbContext
{
    public DbSet<SyncBatch> SyncBatches => Set<SyncBatch>();
    public DbSet<SyncDelta> SyncDeltas => Set<SyncDelta>();
    public DbSet<ConflictRecord> ConflictRecords => Set<ConflictRecord>();
    public DbSet<EntityVersionEntry> EntityVersionEntries => Set<EntityVersionEntry>();

    public SyncDbContext(DbContextOptions<SyncDbContext> options, IDispatcher dispatcher)
        : base(options, dispatcher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SyncDbContext).Assembly);
    }
}
