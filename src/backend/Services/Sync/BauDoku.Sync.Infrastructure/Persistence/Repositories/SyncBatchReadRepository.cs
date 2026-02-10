using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class SyncBatchReadRepository : ISyncBatchReadRepository
{
    private readonly SyncDbContext _context;

    public SyncBatchReadRepository(SyncDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConflictDto>> GetConflictsAsync(
        DeviceId? deviceId,
        ConflictStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SyncBatches
            .Include(b => b.Conflicts)
            .AsNoTracking()
            .AsQueryable();

        if (deviceId is not null)
            query = query.Where(b => b.DeviceId == deviceId);

        var batches = await query.ToListAsync(cancellationToken);

        var conflicts = batches
            .SelectMany(b => b.Conflicts)
            .Where(c => status is null || c.Status == status)
            .Select(c => new ConflictDto(
                c.Id.Value,
                c.EntityRef.EntityType.Value,
                c.EntityRef.EntityId,
                c.ClientPayload.Value,
                c.ServerPayload.Value,
                c.ClientVersion.Value,
                c.ServerVersion.Value,
                c.Status.Value,
                c.DetectedAt))
            .ToList();

        return conflicts;
    }
}
