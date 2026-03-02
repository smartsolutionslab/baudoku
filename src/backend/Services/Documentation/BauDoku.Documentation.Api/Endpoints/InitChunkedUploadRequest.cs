namespace BauDoku.Documentation.Api.Endpoints;

public sealed record InitChunkedUploadRequest(
    Guid InstallationId,
    string FileName,
    string ContentType,
    long TotalSize,
    int TotalChunks,
    string PhotoType,
    string? Caption,
    string? Description,
    GpsPositionRequest? Position);
