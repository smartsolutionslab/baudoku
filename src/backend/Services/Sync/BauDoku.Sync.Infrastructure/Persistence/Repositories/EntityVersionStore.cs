using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class EntityVersionStore : IEntityVersionStore, IEntityVersionReadStore
{
    private readonly SyncDbContext _context;

    public EntityVersionStore(SyncDbContext context)
    {
        _context = context;
    }

    public async Task<SyncVersion> GetCurrentVersionAsync(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        var entry = await _context.EntityVersionEntries
            .FirstOrDefaultAsync(e => e.EntityType == entityType.Value && e.EntityId == entityId, cancellationToken);

        return entry is not null ? new SyncVersion(entry.Version) : SyncVersion.Initial;
    }

    public async Task<string?> GetCurrentPayloadAsync(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        var entry = await _context.EntityVersionEntries
            .FirstOrDefaultAsync(e => e.EntityType == entityType.Value && e.EntityId == entityId, cancellationToken);

        return entry?.Payload;
    }

    public async Task SetVersionAsync(
        EntityType entityType,
        Guid entityId,
        SyncVersion version,
        string payload,
        DeviceId deviceId,
        CancellationToken cancellationToken = default)
    {
        var entry = await _context.EntityVersionEntries
            .FirstOrDefaultAsync(e => e.EntityType == entityType.Value && e.EntityId == entityId, cancellationToken);

        if (entry is not null)
        {
            entry.Version = version.Value;
            entry.Payload = payload;
            entry.LastModified = DateTime.UtcNow;
            entry.LastDeviceId = deviceId.Value;
        }
        else
        {
            entry = new EntityVersionEntry
            {
                EntityType = entityType.Value,
                EntityId = entityId,
                Version = version.Value,
                Payload = payload,
                LastModified = DateTime.UtcNow,
                LastDeviceId = deviceId.Value
            };
            await _context.EntityVersionEntries.AddAsync(entry, cancellationToken);
        }
    }

    public async Task<List<ServerDeltaDto>> GetChangedSinceAsync(
        DateTime? since,
        DeviceId? excludeDeviceId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = _context.EntityVersionEntries.AsNoTracking().AsQueryable();

        if (since is not null)
            query = query.Where(e => e.LastModified > since.Value);

        if (excludeDeviceId is not null)
            query = query.Where(e => e.LastDeviceId != excludeDeviceId.Value);

        return await query
            .OrderBy(e => e.LastModified)
            .Take(limit)
            .Select(e => new ServerDeltaDto(
                e.EntityType,
                e.EntityId,
                "update",
                e.Version,
                e.Payload,
                e.LastModified))
            .ToListAsync(cancellationToken);
    }
}
