using BauDoku.BuildingBlocks.Application.Dispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BauDoku.Sync.Infrastructure.Persistence;

public sealed class SyncDbContextFactory : IDesignTimeDbContextFactory<SyncDbContext>
{
    public SyncDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SyncDbContext>()
            .UseNpgsql("Host=localhost;Database=baudoku_sync;Username=postgres;Password=postgres")
            .Options;

        return new SyncDbContext(options, new NullDispatcher());
    }
}
