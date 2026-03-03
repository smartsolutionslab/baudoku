using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.ReadModel;

public interface ISyncBatchReadRepository
{
    Task<List<ConflictDto>> GetConflictsAsync(DeviceIdentifier? deviceId, ConflictStatus? status, CancellationToken cancellationToken = default);
}
