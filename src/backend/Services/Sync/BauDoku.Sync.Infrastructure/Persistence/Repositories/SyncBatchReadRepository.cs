using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Mapping;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class SyncBatchReadRepository(SyncDbContext context) : ISyncBatchReadRepository
{
    public async Task<List<ConflictDto>> GetConflictsAsync(
        DeviceIdentifier? deviceId,
        ConflictStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = context.SyncBatches
            .Include(b => b.Conflicts)
            .AsNoTracking()
            .AsQueryable();

        if (deviceId is not null)
            query = query.Where(b => b.DeviceId == deviceId);

        var batches = await query.ToListAsync(cancellationToken);

        var conflicts = batches
            .SelectMany(b => b.Conflicts)
            .Where(c => status is null || c.Status == status)
            .Select(c => c.ToDto())
            .ToList();

        return conflicts;
    }
}
