using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Infrastructure.Persistence;
using BauDoku.Documentation.Infrastructure.ReadModel;
using Marten;
using Marten.Events;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Weasel.Core;

namespace BauDoku.Documentation.IntegrationTests.Fixtures;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgis/postgis:17-3.5-alpine").Build();

    public string ConnectionString => container.GetConnectionString();
    public IDocumentStore Store { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        // Create read model schema FIRST (EnsureCreatedAsync is a no-op if any tables exist)
        await using var readModelDb = CreateReadModelDbContext();
        await readModelDb.Database.EnsureCreatedAsync();

        // Create Marten event store (no projections needed for persistence tests)
        Store = DocumentStore.For(options =>
        {
            MartenConfiguration.Configure(options, ConnectionString);
            options.AutoCreateSchemaObjects = AutoCreate.All;
        });

        await Store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
    }

    public ReadModelDbContext CreateReadModelDbContext()
    {
        var options = new DbContextOptionsBuilder<ReadModelDbContext>()
            .UseNpgsql(ConnectionString, o => o.UseNetTopologySuite())
            .Options;

        return new ReadModelDbContext(options);
    }

    public static async Task<Installation?> RehydrateInstallationAsync(IQuerySession session, Guid streamId)
    {
        var events = await session.Events.FetchStreamAsync(streamId);

        if (events.Count == 0)
            return null;

        var domainEvents = events
            .Select(e => e.Data)
            .OfType<IDomainEvent>()
            .ToList();

        var installation = new Installation();
        installation.LoadFromHistory(domainEvents, events[^1].Version);
        return installation;
    }

    public async Task DisposeAsync()
    {
        Store.Dispose();
        await container.DisposeAsync().AsTask();
    }
}
