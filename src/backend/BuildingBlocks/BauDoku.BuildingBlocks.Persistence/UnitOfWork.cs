using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Dispatcher;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Persistence;

namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Persistence;

public sealed class UnitOfWork<TContext>(TContext context, IDispatcher dispatcher) : IUnitOfWork
    where TContext : BaseDbContext
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = context.CollectDomainEvents();
        var result = await context.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
            await dispatcher.Publish(domainEvent, cancellationToken);

        return result;
    }
}
