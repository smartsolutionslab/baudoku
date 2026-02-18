using BauDoku.BuildingBlocks.Domain;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Contracts;

public interface ISyncBatchRepository : IRepository<SyncBatch, SyncBatchIdentifier>
{
    Task<SyncBatch> GetByConflictIdAsync(ConflictRecordIdentifier conflictId, CancellationToken cancellationToken = default);
    Task<List<SyncBatch>> GetPendingBatchesAsync(int limit, CancellationToken cancellationToken = default);
}
