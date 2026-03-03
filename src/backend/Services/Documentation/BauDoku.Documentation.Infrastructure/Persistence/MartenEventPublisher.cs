using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Dispatcher;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Persistence;

public sealed class MartenEventPublisher(IServiceScopeFactory scopeFactory) : DocumentSessionListenerBase
{
    public override async Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken cancellationToken)
    {
        var events = commit.GetEvents().ToList();
        if (events.Count == 0) return;

        using var scope = scopeFactory.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        foreach (var @event in events)
        {
            if (@event.Data is IDomainEvent domainEvent)
            {
                await dispatcher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
