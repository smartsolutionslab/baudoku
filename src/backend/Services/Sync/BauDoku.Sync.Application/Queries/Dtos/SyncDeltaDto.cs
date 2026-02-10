namespace BauDoku.Sync.Application.Queries.Dtos;

public sealed record SyncDeltaDto(
    string EntityType,
    Guid EntityId,
    string Operation,
    long BaseVersion,
    string Payload,
    DateTime Timestamp);
