using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Contracts;

public interface ISyncBatchReadRepository
{
    Task<List<ConflictDto>> GetConflictsAsync(DeviceIdentifier? deviceId, ConflictStatus? status, CancellationToken cancellationToken = default);
}
