using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Infrastructure.Persistence;
using BauDoku.Documentation.Infrastructure.ReadModel;
using BauDoku.Projects.Infrastructure.Persistence;
using BauDoku.ServiceDefaults;
using BauDoku.Sync.Infrastructure.Persistence;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddSingleton<IDispatcher, NoOpDispatcher>();

var projectsCs = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.ProjectsDb);
var documentationCs = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.DocumentationDb);
var syncCs = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.SyncDb);

builder.Services.AddDbContext<ProjectsDbContext>((sp, options) => options.UseNpgsql(projectsCs));

builder.Services.AddMarten(options => MartenConfiguration.Configure(options, documentationCs));

builder.Services.AddDbContext<ReadModelDbContext>(options => options.UseNpgsql(documentationCs, o => o.UseNetTopologySuite()));

builder.Services.AddDbContext<SyncDbContext>((sp, options) => options.UseNpgsql(syncCs));

var host = builder.Build();

try
{
    Log.Information("Starting migration runner...");

    await MigrateAsync<ProjectsDbContext>(host, "Projects");
    await MigrateMartenAsync(host);
    await MigrateAsync<ReadModelDbContext>(host, "Documentation ReadModel");
    await MigrateAsync<SyncDbContext>(host, "Sync");

    Log.Information("All migrations applied successfully.");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Migration runner failed.");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static async Task MigrateAsync<TContext>(IHost host, string contextName) where TContext : DbContext
{
    using var scope = host.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TContext>();

    var pending = await db.Database.GetPendingMigrationsAsync();
    var pendingList = pending.ToList();

    if (pendingList.Count == 0)
    {
        Log.Information("{Context}: No pending migrations", contextName);
        return;
    }

    Log.Information("{Context}: Applying {Count} pending migration(s): {Migrations}",
        contextName, pendingList.Count, string.Join(", ", pendingList));

    await db.Database.MigrateAsync();

    Log.Information("{Context}: Migrations applied successfully", contextName);
}

static async Task MigrateMartenAsync(IHost host)
{
    using var scope = host.Services.CreateScope();
    var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

    Log.Information("Documentation EventStore: Applying Marten schema changes...");
    await store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
    Log.Information("Documentation EventStore: Schema changes applied successfully");
}

sealed class NoOpDispatcher : IDispatcher
{
    public Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Dispatcher is not available in migration runner.");

    public Task Send(ICommand command, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Dispatcher is not available in migration runner.");

    public Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
