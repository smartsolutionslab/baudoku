namespace BauDoku.Sync.Application.Queries.Dtos;

public sealed record SyncBatchDto(
    Guid Id,
    string DeviceId,
    string Status,
    int DeltaCount,
    int ConflictCount,
    DateTime SubmittedAt,
    DateTime? ProcessedAt);
