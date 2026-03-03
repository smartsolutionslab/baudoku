using BauDoku.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.BuildingBlocks.Persistence;

public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    public List<IDomainEvent> CollectDomainEvents()
    {
        var aggregateRoots = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregateRoots.SelectMany(ar => ar.DomainEvents).ToList();

        aggregateRoots.ForEach(ar => ar.ClearDomainEvents());

        return domainEvents;
    }
}
