using System.Linq.Expressions;
using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Domain;
using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class SyncBatchReadRepository(SyncReadDbContext context) : ISyncBatchReadRepository
{
    private readonly DbSet<SyncBatch> syncBatches = context.SyncBatches;

    private static readonly Expression<Func<ConflictRecord, ConflictDto>> toConflictDto = c => new ConflictDto(
        c.Id.Value,
        c.EntityRef.EntityType.Value,
        c.EntityRef.EntityId.Value,
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
        var query = syncBatches.AsQueryable();

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
