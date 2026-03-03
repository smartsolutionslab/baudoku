namespace SmartSolutionsLab.BauDoku.Sync.ReadModel;

public sealed record ConflictDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string ClientPayload,
    string ServerPayload,
    long ClientVersion,
    long ServerVersion,
    string Status,
    DateTime DetectedAt);
