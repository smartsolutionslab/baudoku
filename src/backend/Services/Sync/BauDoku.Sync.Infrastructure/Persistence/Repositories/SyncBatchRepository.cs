using BauDoku.Sync.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class SyncBatchRepository(SyncDbContext context) : ISyncBatchRepository
{
    public async Task<SyncBatch> GetByIdAsync(SyncBatchIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.SyncBatches
            .Include(b => b.Deltas)
            .Include(b => b.Conflicts)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"SyncBatch mit ID '{id.Value}' nicht gefunden.");
    }

    public async Task<SyncBatch> GetByConflictIdAsync(ConflictRecordIdentifier conflictId, CancellationToken cancellationToken = default)
    {
        return await context.SyncBatches
            .Include(b => b.Deltas)
            .Include(b => b.Conflicts)
            .FirstOrDefaultAsync(b => b.Conflicts.Any(c => c.Id == conflictId), cancellationToken)
            ?? throw new KeyNotFoundException($"Batch für Konflikt '{conflictId.Value}' nicht gefunden.");
    }

    public async Task<List<SyncBatch>> GetPendingBatchesAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await context.SyncBatches
            .Include(b => b.Deltas)
            .Include(b => b.Conflicts)
            .Where(b => b.Status == BatchStatus.Pending)
            .OrderBy(b => b.SubmittedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SyncBatch aggregate, CancellationToken cancellationToken = default)
    {
        await context.SyncBatches.AddAsync(aggregate, cancellationToken);
    }
}
