using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Events;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BauDoku.BuildingBlocks.Application.Dispatcher;

public sealed class Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger)
    : IDispatcher
{
    public async Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, cancellationToken);
    }

    public async Task Send(ICommand command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        await handler.Handle((dynamic)command, cancellationToken);
    }

    public async Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)query, cancellationToken);
    }

    public async Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Domain-Event veröffentlicht: {EventType}", domainEvent.GetType().Name);

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            try
            {
                await ((dynamic)handler!).Handle((dynamic)domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler im DomainEventHandler {HandlerType} für {EventType}",
                    handler!.GetType().Name, domainEvent.GetType().Name);
            }
        }
    }
}
