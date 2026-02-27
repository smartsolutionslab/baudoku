using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;
using Marten;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationRepository(IDocumentSession session) : IInstallationRepository
{
    public async Task<Installation> GetByIdAsync(InstallationIdentifier id, CancellationToken cancellationToken = default)
    {
        var events = await session.Events.FetchStreamAsync(id.Value, token: cancellationToken);

        if (events.Count == 0) throw new KeyNotFoundException($"Installation mit ID '{id.Value}' nicht gefunden.");

        var domainEvents = events.Select(e => e.Data).OfType<IDomainEvent>().ToList();

        var installation = new Installation();
        installation.LoadFromHistory(domainEvents, events[^1].Version);

        if (installation.IsDeleted) throw new KeyNotFoundException($"Installation mit ID '{id.Value}' nicht gefunden.");

        return installation;
    }

    public async Task SaveAsync(Installation aggregate, CancellationToken cancellationToken = default)
    {
        var pending = aggregate.DomainEvents.ToArray();
        if (pending.Length == 0) return;

        if (aggregate.Version == 0)
        {
            session.Events.StartStream<Installation>(aggregate.Id.Value, pending);
        }
        else
        {
            session.Events.Append(aggregate.Id.Value, pending);
        }

        await session.SaveChangesAsync(cancellationToken);
        aggregate.ClearDomainEvents();
    }
}
