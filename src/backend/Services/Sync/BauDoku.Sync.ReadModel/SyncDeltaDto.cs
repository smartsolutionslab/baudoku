namespace SmartSolutionsLab.BauDoku.Sync.ReadModel;

public sealed record SyncDeltaDto(
    string EntityType,
    Guid EntityId,
    string Operation,
    long BaseVersion,
    string Payload,
    DateTime Timestamp);
