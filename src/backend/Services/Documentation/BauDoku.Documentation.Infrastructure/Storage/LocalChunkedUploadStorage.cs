using System.Text.Json;
using BauDoku.BuildingBlocks.Infrastructure.Storage;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalChunkedUploadStorage : IChunkedUploadStorage
{
    private readonly LocalStorageDirectory storage;

    public LocalChunkedUploadStorage(IOptions<PhotoStorageOptions> options)
    {
        storage = new LocalStorageDirectory(options.Value.ChunkedPath);
    }

    public Task<UploadSessionIdentifier> InitSessionAsync(ChunkedUploadSession session, CancellationToken ct = default)
    {
        var sessionDir = session.SessionId.ToString();
        storage.CreateSubdirectory(sessionDir);

        var json = JsonSerializer.Serialize(session);
        storage.WriteAllText(Path.Combine(sessionDir, "metadata.json"), json);

        return Task.FromResult(UploadSessionIdentifier.From(session.SessionId));
    }

    public async Task StoreChunkAsync(UploadSessionIdentifier sessionId, int chunkIndex, Stream data, CancellationToken ct = default)
    {
        var sessionDir = sessionId.Value.ToString();
        if (!storage.DirectoryExists(sessionDir))
            throw new InvalidOperationException($"Upload-Session {sessionId.Value} nicht gefunden.");

        await storage.WriteStreamAsync(Path.Combine(sessionDir, $"{chunkIndex}.chunk"), data, ct);
    }

    public Task<ChunkedUploadSession> GetSessionAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var metadataPath = Path.Combine(sessionId.Value.ToString(), "metadata.json");
        if (!storage.FileExists(metadataPath))
            throw new KeyNotFoundException($"Upload-Session mit ID {sessionId.Value} nicht gefunden.");

        var json = storage.ReadAllText(metadataPath);
        var session = JsonSerializer.Deserialize<ChunkedUploadSession>(json)
            ?? throw new KeyNotFoundException($"Upload-Session mit ID {sessionId.Value} nicht gefunden.");
        return Task.FromResult(session);
    }

    public Task<int> GetUploadedChunkCountAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var sessionDir = sessionId.Value.ToString();
        if (!storage.DirectoryExists(sessionDir))
            return Task.FromResult(0);

        var chunkCount = storage.GetFiles(sessionDir, "*.chunk").Length;
        return Task.FromResult(chunkCount);
    }

    public async Task<Stream> AssembleAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var sessionDir = sessionId.Value.ToString();
        var metadataPath = Path.Combine(sessionDir, "metadata.json");
        var json = storage.ReadAllText(metadataPath);
        var session = JsonSerializer.Deserialize<ChunkedUploadSession>(json)
            ?? throw new InvalidOperationException($"Session-Metadaten für {sessionId.Value} nicht lesbar.");

        var assembledPath = Path.Combine(sessionDir, "assembled");
        await using (var assembledStream = storage.OpenWrite(assembledPath))
        {
            for (var i = 0; i < session.TotalChunks; i++)
            {
                var chunkPath = Path.Combine(sessionDir, $"{i}.chunk");
                if (!storage.FileExists(chunkPath))
                    throw new InvalidOperationException($"Chunk {i} für Session {sessionId.Value} nicht gefunden.");

                await using var chunkStream = storage.OpenRead(chunkPath);
                await chunkStream.CopyToAsync(assembledStream, ct);
            }
        }

        return new FileStream(storage.Resolve(assembledPath), FileMode.Open, FileAccess.Read, FileShare.None,
            bufferSize: 4096, FileOptions.DeleteOnClose);
    }

    public Task CleanupSessionAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var sessionDir = sessionId.Value.ToString();
        if (storage.DirectoryExists(sessionDir))
            storage.DeleteDirectory(sessionDir);

        return Task.CompletedTask;
    }
}
