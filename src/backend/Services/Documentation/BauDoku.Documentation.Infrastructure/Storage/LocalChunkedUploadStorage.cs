using BauDoku.BuildingBlocks.Infrastructure.Storage;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalChunkedUploadStorage(IOptions<PhotoStorageOptions> options) : IChunkedUploadStorage
{
    private readonly LocalStorageDirectory storage = new(options.Value.ChunkedPath);

    public async Task<UploadSessionIdentifier> InitSessionAsync(ChunkedUploadSession session, CancellationToken cancellationToken = default)
    {
        var sessionDir = session.SessionId.ToString();
        storage.CreateSubdirectory(sessionDir);

        await storage.WriteJsonAsync(Path.Combine(sessionDir, "metadata.json"), session, cancellationToken);

        return UploadSessionIdentifier.From(session.SessionId);
    }

    public async Task StoreChunkAsync(UploadSessionIdentifier sessionId, int chunkIndex, Stream data, CancellationToken cancellationToken = default)
    {
        var sessionDir = sessionId.Value.ToString();
        if (!storage.DirectoryExists(sessionDir)) throw new InvalidOperationException($"Upload-Session {sessionId.Value} nicht gefunden.");

        await storage.WriteStreamAsync(Path.Combine(sessionDir, $"{chunkIndex}.chunk"), data, cancellationToken);
    }

    public async Task<ChunkedUploadSession> GetSessionAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default)
    {
        var metadataPath = Path.Combine(sessionId.Value.ToString(), "metadata.json");
        if (!storage.FileExists(metadataPath)) throw new KeyNotFoundException($"Upload-Session mit ID {sessionId.Value} nicht gefunden.");

        return await storage.ReadJsonAsync<ChunkedUploadSession>(metadataPath, cancellationToken)
            ?? throw new KeyNotFoundException($"Upload-Session mit ID {sessionId.Value} nicht gefunden.");
    }

    public Task<int> GetUploadedChunkCountAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default)
    {
        var sessionDir = sessionId.Value.ToString();
        if (!storage.DirectoryExists(sessionDir)) return Task.FromResult(0);

        var chunkCount = storage.GetFiles(sessionDir, "*.chunk").Length;
        return Task.FromResult(chunkCount);
    }

    public async Task<Stream> AssembleAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default)
    {
        var sessionDir = sessionId.Value.ToString();
        var metadataPath = Path.Combine(sessionDir, "metadata.json");
        var session = await storage.ReadJsonAsync<ChunkedUploadSession>(metadataPath, cancellationToken) ?? throw new InvalidOperationException($"Session-Metadaten für {sessionId.Value} nicht lesbar.");

        var assembledPath = Path.Combine(sessionDir, "assembled");
        await using (var assembledStream = storage.OpenWrite(assembledPath))
        {
            for (var index = 0; index < session.TotalChunks; index++)
            {
                var chunkPath = Path.Combine(sessionDir, $"{index}.chunk");
                if (!storage.FileExists(chunkPath)) throw new InvalidOperationException($"Chunk {index} für Session {sessionId.Value} nicht gefunden.");

                await using var chunkStream = storage.OpenRead(chunkPath);
                await chunkStream.CopyToAsync(assembledStream, cancellationToken);
            }
        }

        return new FileStream(storage.Resolve(assembledPath), FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, FileOptions.DeleteOnClose);
    }

    public Task CleanupSessionAsync(UploadSessionIdentifier sessionId, CancellationToken cancellationToken = default)
    {
        storage.DeleteDirectory(sessionId.Value.ToString());
        return Task.CompletedTask;
    }
}
