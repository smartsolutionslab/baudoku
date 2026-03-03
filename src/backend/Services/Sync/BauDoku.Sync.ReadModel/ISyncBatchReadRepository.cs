using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.ReadModel;

public interface ISyncBatchReadRepository
{
    Task<List<ConflictDto>> GetConflictsAsync(DeviceIdentifier? deviceId, ConflictStatus? status, CancellationToken cancellationToken = default);
}
