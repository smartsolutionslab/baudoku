using BauDoku.BuildingBlocks.Infrastructure.Storage;
using BauDoku.Documentation.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class ChunkedUploadCleanupService(IOptions<PhotoStorageOptions> options, ILogger<ChunkedUploadCleanupService> logger)
    : BackgroundService
{
    private readonly LocalStorageDirectory storage = new(options.Value.ChunkedPath);
    private readonly TimeSpan interval = TimeSpan.FromMinutes(15);
    private readonly TimeSpan maxAge = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(interval, stoppingToken);
            await CleanupExpiredSessionsAsync(stoppingToken);
        }
    }

    private async Task CleanupExpiredSessionsAsync(CancellationToken ct)
    {
        var sessionDirs = storage.GetSubdirectories();
        var cleaned = 0;

        foreach (var sessionDir in sessionDirs)
        {
            var sessionName = Path.GetFileName(sessionDir);
            var metadataPath = Path.Combine(sessionName, "metadata.json");

            if (!storage.FileExists(metadataPath))
            {
                TryDeleteDirectory(sessionName);
                cleaned++;
                continue;
            }

            try
            {
                var session = await storage.ReadJsonAsync<ChunkedUploadSession>(metadataPath, ct);
                if (session is null || DateTime.UtcNow - session.CreatedAt > maxAge)
                {
                    TryDeleteDirectory(sessionName);
                    cleaned++;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Fehler beim Lesen der Session-Metadaten in {SessionDir}", sessionDir);
                TryDeleteDirectory(sessionName);
                cleaned++;
            }
        }

        if (cleaned > 0)
        {
            logger.LogInformation("Chunked-Upload-Cleanup: {Count} abgelaufene Sessions entfernt", cleaned);
        }
    }

    private void TryDeleteDirectory(string relativePath)
    {
        try
        {
            storage.DeleteDirectory(relativePath);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Fehler beim Löschen des Session-Verzeichnisses {Path}", relativePath);
        }
    }
}
