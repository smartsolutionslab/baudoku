using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Contracts;

public sealed record ChunkedUploadSession(
    Guid SessionId,
    Guid InstallationId,
    string FileName,
    string ContentType,
    long TotalSize,
    int TotalChunks,
    string PhotoType,
    string? Caption,
    string? Description,
    GpsPositionDto? Position,
    DateTime CreatedAt);
