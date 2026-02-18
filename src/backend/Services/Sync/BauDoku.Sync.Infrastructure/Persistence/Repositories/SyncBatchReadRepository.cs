using System.Linq.Expressions;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.Entities;
using BauDoku.Sync.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class SyncBatchReadRepository(SyncDbContext context) : ISyncBatchReadRepository
{
    private static readonly Expression<Func<ConflictRecord, ConflictDto>> toConflictDto = c => new ConflictDto(
        c.Id.Value,
        c.EntityRef.EntityType.Value,
        c.EntityRef.EntityId,
        c.ClientPayload.Value,
        c.ServerPayload.Value,
        c.ClientVersion.Value,
        c.ServerVersion.Value,
        c.Status.Value,
        c.DetectedAt);

    public async Task<List<ConflictDto>> GetConflictsAsync(
        DeviceIdentifier? deviceId,
        ConflictStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = context.SyncBatches.AsNoTracking().AsQueryable();

        if (deviceId is not null)
            query = query.Where(b => b.DeviceId == deviceId);

        var conflictsQuery = query.SelectMany(b => b.Conflicts);

        if (status is not null)
            conflictsQuery = conflictsQuery.Where(c => c.Status == status);

        return await conflictsQuery
            .Select(toConflictDto)
            .ToListAsync(cancellationToken);
    }
}
