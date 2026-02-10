using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Projects.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace BauDoku.Projects.IntegrationTests.Fixtures;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public ProjectsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProjectsDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        var dispatcher = Substitute.For<IDispatcher>();
        return new ProjectsDbContext(options, dispatcher);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync().AsTask();
    }
}
