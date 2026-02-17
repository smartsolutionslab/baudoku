using System.Text.Json;
using BauDoku.Documentation.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class ChunkedUploadCleanupService(IOptions<PhotoStorageOptions> options, ILogger<ChunkedUploadCleanupService> logger)
    : BackgroundService
{
    private readonly string basePath = ResolveBasePath(options.Value.ChunkedPath);
    private readonly TimeSpan interval = TimeSpan.FromMinutes(15);
    private readonly TimeSpan maxAge = TimeSpan.FromHours(1);

    private static string ResolveBasePath(string chunkedPath)
    {
        if (Path.IsPathRooted(chunkedPath))
            return chunkedPath;
        return Path.Combine(Directory.GetCurrentDirectory(), chunkedPath);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(interval, stoppingToken);
            CleanupExpiredSessions();
        }
    }

    private void CleanupExpiredSessions()
    {
        if (!Directory.Exists(basePath))
            return;

        var sessionDirs = Directory.GetDirectories(basePath);
        var cleaned = 0;

        foreach (var sessionDir in sessionDirs)
        {
            var metadataPath = Path.Combine(sessionDir, "metadata.json");
            if (!File.Exists(metadataPath))
            {
                TryDeleteDirectory(sessionDir);
                cleaned++;
                continue;
            }

            try
            {
                var json = File.ReadAllText(metadataPath);
                var session = JsonSerializer.Deserialize<ChunkedUploadSession>(json);
                if (session is null || DateTime.UtcNow - session.CreatedAt > maxAge)
                {
                    TryDeleteDirectory(sessionDir);
                    cleaned++;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Fehler beim Lesen der Session-Metadaten in {SessionDir}", sessionDir);
                TryDeleteDirectory(sessionDir);
                cleaned++;
            }
        }

        if (cleaned > 0)
        {
            logger.LogInformation("Chunked-Upload-Cleanup: {Count} abgelaufene Sessions entfernt", cleaned);
        }
    }

    private void TryDeleteDirectory(string path)
    {
        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Fehler beim Löschen des Session-Verzeichnisses {Path}", path);
        }
    }
}
