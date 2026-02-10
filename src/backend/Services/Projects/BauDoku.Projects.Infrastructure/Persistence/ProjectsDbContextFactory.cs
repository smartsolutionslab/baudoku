using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BauDoku.Projects.Infrastructure.Persistence;

public sealed class ProjectsDbContextFactory : IDesignTimeDbContextFactory<ProjectsDbContext>
{
    public ProjectsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ProjectsDbContext>()
            .UseNpgsql("Host=localhost;Database=baudoku_projects;Username=postgres;Password=postgres")
            .Options;

        return new ProjectsDbContext(options, new NullDispatcher());
    }

    private sealed class NullDispatcher : IDispatcher
    {
        public Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task Send(ICommand command, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
