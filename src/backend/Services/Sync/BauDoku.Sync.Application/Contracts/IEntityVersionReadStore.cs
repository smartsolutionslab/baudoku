using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Contracts;

public interface IEntityVersionReadStore
{
    Task<List<ServerDeltaDto>> GetChangedSinceAsync(
        DateTime? since,
        DeviceIdentifier? excludeDeviceId,
        int limit,
        CancellationToken cancellationToken = default);
}
