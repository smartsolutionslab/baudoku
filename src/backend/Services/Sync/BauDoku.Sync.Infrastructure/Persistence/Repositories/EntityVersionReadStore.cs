using System.Linq.Expressions;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.ReadModel;
using BauDoku.Sync.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class EntityVersionReadStore(SyncReadDbContext context) : IEntityVersionReadStore
{
    private static readonly Expression<Func<EntityVersionEntry, ServerDeltaDto>> toServerDelta = e => new ServerDeltaDto(
        e.EntityType,
        e.EntityId,
        "update",
        e.Version,
        e.Payload,
        e.LastModified);

    public async Task<List<ServerDeltaDto>> GetChangedSinceAsync(
        DateTime? since,
        DeviceIdentifier? excludeDeviceId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = context.EntityVersionEntries.AsQueryable();

        if (since is not null)
            query = query.Where(e => e.LastModified > since.Value);

        if (excludeDeviceId is not null)
            query = query.Where(e => e.LastDeviceId != excludeDeviceId.Value);

        return await query
            .OrderBy(e => e.LastModified)
            .Take(limit)
            .Select(toServerDelta)
            .ToListAsync(cancellationToken);
    }
}
