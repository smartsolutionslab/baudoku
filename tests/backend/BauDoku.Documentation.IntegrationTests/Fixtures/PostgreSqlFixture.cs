using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace BauDoku.Documentation.IntegrationTests.Fixtures;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgis/postgis:17-3.5-alpine")
        .Build();

    public string ConnectionString => container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public DocumentationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DocumentationDbContext>()
            .UseNpgsql(ConnectionString, o => o.UseNetTopologySuite())
            .Options;

        var dispatcher = Substitute.For<IDispatcher>();
        return new DocumentationDbContext(options, dispatcher);
    }

    public async Task DisposeAsync()
    {
        await container.DisposeAsync().AsTask();
    }
}
