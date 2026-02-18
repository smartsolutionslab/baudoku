using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Contracts;

public interface IEntityVersionStore
{
    Task<SyncVersion> GetCurrentVersionAsync(
        EntityReference entityRef,
        CancellationToken cancellationToken = default);

    Task<string?> GetCurrentPayloadAsync(
        EntityReference entityRef,
        CancellationToken cancellationToken = default);

    Task SetVersionAsync(
        EntityReference entityRef,
        SyncVersion version,
        string payload,
        DeviceIdentifier deviceId,
        CancellationToken cancellationToken = default);
}
