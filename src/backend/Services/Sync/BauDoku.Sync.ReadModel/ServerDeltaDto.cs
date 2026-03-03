namespace BauDoku.Sync.ReadModel;

public sealed record ServerDeltaDto(
    string EntityType,
    Guid EntityId,
    string Operation,
    long Version,
    string Payload,
    DateTime Timestamp);
