using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.Documentation.Infrastructure.Persistence;
using BauDoku.Documentation.Infrastructure.ReadModel;
using BauDoku.Projects.Infrastructure.Persistence;
using BauDoku.Sync.Infrastructure.Persistence;
using Marten;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NSubstitute;
using Testcontainers.PostgreSql;
using Weasel.Core;

namespace BauDoku.E2E.SmokeTests.Fixtures;

public sealed class E2EFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgis/postgis:17-3.5-alpine")
        .Build();

    private IDocumentStore? documentationStore;

    public string ProjectsConnectionString { get; private set; } = default!;
    public string DocumentationConnectionString { get; private set; } = default!;
    public string SyncConnectionString { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        var defaultConnStr = container.GetConnectionString();

        await using (var conn = new NpgsqlConnection(defaultConnStr))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("CREATE DATABASE projects_e2e", conn))
                await cmd.ExecuteNonQueryAsync();

            await using (var cmd = new NpgsqlCommand("CREATE DATABASE documentation_e2e", conn))
                await cmd.ExecuteNonQueryAsync();

            await using (var cmd = new NpgsqlCommand("CREATE DATABASE sync_e2e", conn))
                await cmd.ExecuteNonQueryAsync();
        }

        var builder = new NpgsqlConnectionStringBuilder(defaultConnStr)
        {
            Database = "projects_e2e"
        };

        ProjectsConnectionString = builder.ConnectionString;

        builder.Database = "documentation_e2e";
        DocumentationConnectionString = builder.ConnectionString;

        builder.Database = "sync_e2e";
        SyncConnectionString = builder.ConnectionString;

        var dispatcher = Substitute.For<IDispatcher>();

        // Projects BC: EF Core
        var projectsOptions = new DbContextOptionsBuilder<ProjectsDbContext>()
            .UseNpgsql(ProjectsConnectionString)
            .Options;
        await using (var ctx = new ProjectsDbContext(projectsOptions, dispatcher))
            await ctx.Database.EnsureCreatedAsync();

        // Documentation BC: ReadModelDbContext FIRST (EnsureCreatedAsync is a no-op if any tables exist)
        var readModelOptions = new DbContextOptionsBuilder<ReadModelDbContext>()
            .UseNpgsql(DocumentationConnectionString, o => o.UseNetTopologySuite())
            .Options;
        await using (var ctx = new ReadModelDbContext(readModelOptions))
            await ctx.Database.EnsureCreatedAsync();

        // Documentation BC: Marten event store
        documentationStore = DocumentStore.For(options =>
        {
            MartenConfiguration.Configure(options, DocumentationConnectionString);
            options.AutoCreateSchemaObjects = AutoCreate.All;
        });

        await documentationStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        // Sync BC: EF Core
        var syncOptions = new DbContextOptionsBuilder<SyncDbContext>()
            .UseNpgsql(SyncConnectionString)
            .Options;
        await using (var ctx = new SyncDbContext(syncOptions, dispatcher))
            await ctx.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        documentationStore?.Dispose();
        await container.DisposeAsync().AsTask();
    }
}
