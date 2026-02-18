using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Contracts;

public interface IChunkedUploadStorage
{
    Task<UploadSessionIdentifier> InitSessionAsync(ChunkedUploadSession session, CancellationToken ct = default);
    Task StoreChunkAsync(UploadSessionIdentifier sessionId, int chunkIndex, Stream data, CancellationToken ct = default);
    Task<ChunkedUploadSession?> GetSessionAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default);
    Task<int> GetUploadedChunkCountAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default);
    Task<Stream> AssembleAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default);
    Task CleanupSessionAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default);
}
