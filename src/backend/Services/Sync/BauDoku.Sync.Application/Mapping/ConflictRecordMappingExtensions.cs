using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Mapping;

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
