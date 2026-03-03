using BauDoku.Sync.Application.ReadModel;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Mapping;

public static class ConflictRecordMappingExtensions
{
    public static ConflictDto ToDto(this ConflictRecord conflict) =>
        new(conflict.Id.Value,
            conflict.EntityRef.EntityType.Value,
            conflict.EntityRef.EntityId.Value,
            conflict.ClientPayload.Value,
            conflict.ServerPayload.Value,
            conflict.ClientVersion.Value,
            conflict.ServerVersion.Value,
            conflict.Status.Value,
            conflict.DetectedAt);
}
