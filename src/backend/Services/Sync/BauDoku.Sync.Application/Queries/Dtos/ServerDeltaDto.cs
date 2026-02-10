namespace BauDoku.Sync.Application.Queries.Dtos;

public sealed record ServerDeltaDto(
    string EntityType,
    Guid EntityId,
    string Operation,
    long Version,
    string Payload,
    DateTime Timestamp);
