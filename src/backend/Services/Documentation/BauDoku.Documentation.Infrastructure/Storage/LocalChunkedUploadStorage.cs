using System.Text.Json;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalChunkedUploadStorage : IChunkedUploadStorage
{
    private readonly string basePath;

    public LocalChunkedUploadStorage(IOptions<PhotoStorageOptions> options)
    {
        basePath = options.Value.ChunkedPath;
        if (!Path.IsPathRooted(basePath))
            basePath = Path.Combine(Directory.GetCurrentDirectory(), basePath);
        Directory.CreateDirectory(basePath);
    }

    public Task<UploadSessionIdentifier> InitSessionAsync(ChunkedUploadSession session, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, session.SessionId.ToString());
        Directory.CreateDirectory(sessionDir);

        var metadataPath = Path.Combine(sessionDir, "metadata.json");
        var json = JsonSerializer.Serialize(session);
        File.WriteAllText(metadataPath, json);

        return Task.FromResult(UploadSessionIdentifier.From(session.SessionId));
    }

    public async Task StoreChunkAsync(UploadSessionIdentifier sessionId, int chunkIndex, Stream data, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.Value.ToString());
        if (!Directory.Exists(sessionDir))
            throw new InvalidOperationException($"Upload-Session {sessionId.Value} nicht gefunden.");

        var chunkPath = Path.Combine(sessionDir, $"{chunkIndex}.chunk");
        await using var fileStream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write);
        await data.CopyToAsync(fileStream, ct);
    }

    public Task<ChunkedUploadSession?> GetSessionAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var metadataPath = Path.Combine(basePath, sessionId.Value.ToString(), "metadata.json");
        if (!File.Exists(metadataPath))
            return Task.FromResult<ChunkedUploadSession?>(null);

        var json = File.ReadAllText(metadataPath);
        var session = JsonSerializer.Deserialize<ChunkedUploadSession>(json);
        return Task.FromResult(session);
    }

    public Task<int> GetUploadedChunkCountAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.Value.ToString());
        if (!Directory.Exists(sessionDir))
            return Task.FromResult(0);

        var chunkCount = Directory.GetFiles(sessionDir, "*.chunk").Length;
        return Task.FromResult(chunkCount);
    }

    public async Task<Stream> AssembleAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.Value.ToString());
        var metadataPath = Path.Combine(sessionDir, "metadata.json");
        var json = File.ReadAllText(metadataPath);
        var session = JsonSerializer.Deserialize<ChunkedUploadSession>(json)
            ?? throw new InvalidOperationException($"Session-Metadaten für {sessionId.Value} nicht lesbar.");

        var assembledPath = Path.Combine(sessionDir, "assembled");
        await using (var assembledStream = new FileStream(assembledPath, FileMode.Create, FileAccess.Write))
        {
            for (var i = 0; i < session.TotalChunks; i++)
            {
                var chunkPath = Path.Combine(sessionDir, $"{i}.chunk");
                if (!File.Exists(chunkPath))
                    throw new InvalidOperationException($"Chunk {i} für Session {sessionId.Value} nicht gefunden.");

                await using var chunkStream = new FileStream(chunkPath, FileMode.Open, FileAccess.Read);
                await chunkStream.CopyToAsync(assembledStream, ct);
            }
        }

        return new FileStream(assembledPath, FileMode.Open, FileAccess.Read, FileShare.None,
            bufferSize: 4096, FileOptions.DeleteOnClose);
    }

    public Task CleanupSessionAsync(UploadSessionIdentifier sessionId, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.Value.ToString());
        if (Directory.Exists(sessionDir))
            Directory.Delete(sessionDir, recursive: true);

        return Task.CompletedTask;
    }
}
