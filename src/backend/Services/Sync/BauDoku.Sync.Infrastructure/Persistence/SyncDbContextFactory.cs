using BauDoku.BuildingBlocks.Application.Dispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BauDoku.Sync.Infrastructure.Persistence;

public sealed class SyncDbContextFactory : IDesignTimeDbContextFactory<SyncDbContext>
{
    public SyncDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SyncDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=baudoku_sync;Username=postgres;Password=postgres");

        return new SyncDbContext(optionsBuilder.Options, new NoOpDispatcher());
    }

    private sealed class NoOpDispatcher : IDispatcher
    {
        public Task<TResult> Send<TResult>(BuildingBlocks.Application.Commands.ICommand<TResult> command, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task Send(BuildingBlocks.Application.Commands.ICommand command, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResult> Query<TResult>(BuildingBlocks.Application.Queries.IQuery<TResult> query, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task Publish(BuildingBlocks.Domain.IDomainEvent domainEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
