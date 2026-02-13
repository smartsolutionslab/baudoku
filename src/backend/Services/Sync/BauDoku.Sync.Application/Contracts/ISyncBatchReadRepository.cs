using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Contracts;

public interface ISyncBatchReadRepository
{
    Task<List<ConflictDto>> GetConflictsAsync(
        DeviceIdentifier? deviceId,
        ConflictStatus? status,
        CancellationToken cancellationToken = default);
}
