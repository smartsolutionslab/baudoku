using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Contracts;

public interface IChunkedUploadStorage
{
    Task<UploadSessionIdentifier> InitSessionAsync(ChunkedUploadSession session, CancellationToken cancellationToken = default);
    Task StoreChunkAsync(UploadSessionIdentifier sessionId, int chunkIndex, Stream data, CancellationToken cancellationToken = default);
    Task<ChunkedUploadSession> GetSessionAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default);
    Task<int> GetUploadedChunkCountAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default);
    Task<Stream> AssembleAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default);
    Task CleanupSessionAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default);
}
