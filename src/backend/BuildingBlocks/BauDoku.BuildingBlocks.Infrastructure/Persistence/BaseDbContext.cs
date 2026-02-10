using BauDoku.BuildingBlocks.Application.Dispatcher;
using BauDoku.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.BuildingBlocks.Infrastructure.Persistence;

public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    private readonly IDispatcher _dispatcher;

    protected BaseDbContext(DbContextOptions options, IDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = GetDomainEvents();
        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _dispatcher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot<ValueObject>>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(ar => ar.DomainEvents)
            .ToList();

        aggregateRoots.ForEach(ar => ar.ClearDomainEvents());

        return domainEvents;
    }
}
