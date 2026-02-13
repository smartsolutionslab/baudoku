using System.Text.Json;
using BauDoku.Documentation.Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalChunkedUploadStorage : IChunkedUploadStorage
{
    private readonly string basePath;

    public LocalChunkedUploadStorage(IConfiguration configuration)
    {
        basePath = configuration["PhotoStorage:ChunkedPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads", "chunks");
        Directory.CreateDirectory(basePath);
    }

    public Task<Guid> InitSessionAsync(ChunkedUploadSession session, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, session.SessionId.ToString());
        Directory.CreateDirectory(sessionDir);

        var metadataPath = Path.Combine(sessionDir, "metadata.json");
        var json = JsonSerializer.Serialize(session);
        File.WriteAllText(metadataPath, json);

        return Task.FromResult(session.SessionId);
    }

    public async Task StoreChunkAsync(Guid sessionId, int chunkIndex, Stream data, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.ToString());
        if (!Directory.Exists(sessionDir))
            throw new InvalidOperationException($"Upload-Session {sessionId} nicht gefunden.");

        var chunkPath = Path.Combine(sessionDir, $"{chunkIndex}.chunk");
        await using var fileStream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write);
        await data.CopyToAsync(fileStream, ct);
    }

    public Task<ChunkedUploadSession?> GetSessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        var metadataPath = Path.Combine(basePath, sessionId.ToString(), "metadata.json");
        if (!File.Exists(metadataPath))
            return Task.FromResult<ChunkedUploadSession?>(null);

        var json = File.ReadAllText(metadataPath);
        var session = JsonSerializer.Deserialize<ChunkedUploadSession>(json);
        return Task.FromResult(session);
    }

    public Task<int> GetUploadedChunkCountAsync(Guid sessionId, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.ToString());
        if (!Directory.Exists(sessionDir))
            return Task.FromResult(0);

        var chunkCount = Directory.GetFiles(sessionDir, "*.chunk").Length;
        return Task.FromResult(chunkCount);
    }

    public async Task<Stream> AssembleAsync(Guid sessionId, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.ToString());
        var metadataPath = Path.Combine(sessionDir, "metadata.json");
        var json = File.ReadAllText(metadataPath);
        var session = JsonSerializer.Deserialize<ChunkedUploadSession>(json)
            ?? throw new InvalidOperationException($"Session-Metadaten für {sessionId} nicht lesbar.");

        var assembledPath = Path.Combine(sessionDir, "assembled");
        await using (var assembledStream = new FileStream(assembledPath, FileMode.Create, FileAccess.Write))
        {
            for (var i = 0; i < session.TotalChunks; i++)
            {
                var chunkPath = Path.Combine(sessionDir, $"{i}.chunk");
                if (!File.Exists(chunkPath))
                    throw new InvalidOperationException($"Chunk {i} für Session {sessionId} nicht gefunden.");

                await using var chunkStream = new FileStream(chunkPath, FileMode.Open, FileAccess.Read);
                await chunkStream.CopyToAsync(assembledStream, ct);
            }
        }

        return new FileStream(assembledPath, FileMode.Open, FileAccess.Read, FileShare.None,
            bufferSize: 4096, FileOptions.DeleteOnClose);
    }

    public Task CleanupSessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        var sessionDir = Path.Combine(basePath, sessionId.ToString());
        if (Directory.Exists(sessionDir))
            Directory.Delete(sessionDir, recursive: true);

        return Task.CompletedTask;
    }
}
