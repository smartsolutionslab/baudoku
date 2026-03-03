using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.Application.Dispatcher;

public sealed class NullDispatcher : IDispatcher
{
    public Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task Send(ICommand command, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
