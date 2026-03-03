using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public interface ISyncBatchRepository : IRepository<SyncBatch, SyncBatchIdentifier>
{
    Task<SyncBatch> GetByConflictIdAsync(ConflictRecordIdentifier conflictId, CancellationToken cancellationToken = default);
    Task<List<SyncBatch>> GetPendingBatchesAsync(int limit, CancellationToken cancellationToken = default);
}
