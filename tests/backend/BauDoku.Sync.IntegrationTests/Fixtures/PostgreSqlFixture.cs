using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Sync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace BauDoku.Sync.IntegrationTests.Fixtures;

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

    public SyncDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SyncDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        var dispatcher = Substitute.For<IDispatcher>();
        return new SyncDbContext(options, dispatcher);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync().AsTask();
    }
}
