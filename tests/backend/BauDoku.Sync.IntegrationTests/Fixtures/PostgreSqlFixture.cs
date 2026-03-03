using SmartSolutionsLab.BauDoku.Sync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace SmartSolutionsLab.BauDoku.Sync.IntegrationTests.Fixtures;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public string ConnectionString => container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await container.StartAsync();
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public SyncDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SyncDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new SyncDbContext(options);
    }

    public SyncReadDbContext CreateReadContext()
    {
        var options = new DbContextOptionsBuilder<SyncReadDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new SyncReadDbContext(options);
    }

    public async Task DisposeAsync()
    {
        await container.DisposeAsync().AsTask();
    }
}
