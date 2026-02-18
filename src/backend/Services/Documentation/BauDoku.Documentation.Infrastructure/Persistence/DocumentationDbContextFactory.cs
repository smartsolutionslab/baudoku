using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BauDoku.Documentation.Infrastructure.Persistence;

public sealed class DocumentationDbContextFactory : IDesignTimeDbContextFactory<DocumentationDbContext>
{
    public DocumentationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DocumentationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=baudoku_documentation;Username=postgres;Password=postgres", o => o.UseNetTopologySuite());

        return new DocumentationDbContext(optionsBuilder.Options, new NullDispatcher());
    }

    private sealed class NullDispatcher : IDispatcher
    {
        public Task<TResult> Send<TResult>(BuildingBlocks.Application.Commands.ICommand<TResult> command, CancellationToken ct = default)
            => throw new NotSupportedException("Design-time only.");

        public Task Send(BuildingBlocks.Application.Commands.ICommand command, CancellationToken ct = default)
            => throw new NotSupportedException("Design-time only.");

        public Task<TResult> Query<TResult>(BuildingBlocks.Application.Queries.IQuery<TResult> query, CancellationToken ct = default)
            => throw new NotSupportedException("Design-time only.");

        public Task Publish(IDomainEvent domainEvent, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
