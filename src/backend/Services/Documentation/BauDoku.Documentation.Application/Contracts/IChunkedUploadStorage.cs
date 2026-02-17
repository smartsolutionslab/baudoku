namespace BauDoku.Documentation.Application.Contracts;

public interface IChunkedUploadStorage
{
    Task<Guid> InitSessionAsync(ChunkedUploadSession session, CancellationToken ct = default);
    Task StoreChunkAsync(Guid sessionId, int chunkIndex, Stream data, CancellationToken ct = default);
    Task<ChunkedUploadSession?> GetSessionAsync(Guid sessionId, CancellationToken ct = default);
    Task<int> GetUploadedChunkCountAsync(Guid sessionId, CancellationToken ct = default);
    Task<Stream> AssembleAsync(Guid sessionId, CancellationToken ct = default);
    Task CleanupSessionAsync(Guid sessionId, CancellationToken ct = default);
}
