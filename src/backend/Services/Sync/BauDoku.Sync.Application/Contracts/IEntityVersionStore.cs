using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Contracts;

public interface IEntityVersionStore
{
    Task<SyncVersion> GetCurrentVersionAsync(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);

    Task<string?> GetCurrentPayloadAsync(
        EntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);

    Task SetVersionAsync(
        EntityType entityType,
        Guid entityId,
        SyncVersion version,
        string payload,
        DeviceIdentifier deviceId,
        CancellationToken cancellationToken = default);
}
