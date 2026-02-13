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

public interface IChunkedUploadStorage
{
    Task<Guid> InitSessionAsync(ChunkedUploadSession session, CancellationToken ct = default);
    Task StoreChunkAsync(Guid sessionId, int chunkIndex, Stream data, CancellationToken ct = default);
    Task<ChunkedUploadSession?> GetSessionAsync(Guid sessionId, CancellationToken ct = default);
    Task<int> GetUploadedChunkCountAsync(Guid sessionId, CancellationToken ct = default);
    Task<Stream> AssembleAsync(Guid sessionId, CancellationToken ct = default);
    Task CleanupSessionAsync(Guid sessionId, CancellationToken ct = default);
}
