using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Contracts;

public interface ISyncBatchRepository : IRepository<SyncBatch, SyncBatchId>
{
    Task<SyncBatch?> GetByConflictIdAsync(ConflictRecordId conflictId, CancellationToken cancellationToken = default);
    Task<List<SyncBatch>> GetPendingBatchesAsync(int limit, CancellationToken cancellationToken = default);
}
