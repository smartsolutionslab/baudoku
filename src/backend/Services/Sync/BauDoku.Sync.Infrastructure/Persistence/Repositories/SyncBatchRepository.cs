using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Sync.Infrastructure.Persistence.Repositories;

public sealed class SyncBatchRepository : ISyncBatchRepository
{
    private readonly SyncDbContext _context;

    public SyncBatchRepository(SyncDbContext context)
    {
        _context = context;
    }

    public async Task<SyncBatch?> GetByIdAsync(SyncBatchId id, CancellationToken cancellationToken = default)
    {
        return await _context.SyncBatches
            .Include(b => b.Deltas)
            .Include(b => b.Conflicts)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<SyncBatch?> GetByConflictIdAsync(ConflictRecordId conflictId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncBatches
            .Include(b => b.Deltas)
            .Include(b => b.Conflicts)
            .FirstOrDefaultAsync(b => b.Conflicts.Any(c => c.Id == conflictId), cancellationToken);
    }

    public async Task AddAsync(SyncBatch aggregate, CancellationToken cancellationToken = default)
    {
        await _context.SyncBatches.AddAsync(aggregate, cancellationToken);
    }
}
