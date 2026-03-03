using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Contracts;

public interface IEntityVersionReadStore
{
    Task<List<ServerDeltaDto>> GetChangedSinceAsync(DateTime? since, DeviceIdentifier? excludeDeviceId, int limit, CancellationToken cancellationToken = default);
}
