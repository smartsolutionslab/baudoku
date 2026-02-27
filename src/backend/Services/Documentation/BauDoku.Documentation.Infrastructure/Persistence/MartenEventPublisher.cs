using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Domain;
using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Documentation.Infrastructure.Persistence;

public sealed class MartenEventPublisher(IServiceScopeFactory scopeFactory) : DocumentSessionListenerBase
{
    public override async Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        var events = commit.GetEvents();
        if (!events.Any()) return;

        using var scope = scopeFactory.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        foreach (var @event in events)
        {
            if (@event.Data is IDomainEvent domainEvent)
            {
                await dispatcher.Publish(domainEvent, token);
            }
        }
    }
}
