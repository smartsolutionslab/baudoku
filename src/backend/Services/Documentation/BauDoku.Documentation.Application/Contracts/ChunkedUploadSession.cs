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
    double? Latitude,
    double? Longitude,
    double? Altitude,
    double? HorizontalAccuracy,
    string? GpsSource,
    DateTime CreatedAt);
