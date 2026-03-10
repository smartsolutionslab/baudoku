using SmartSolutionsLab.BauDoku.BuildingBlocks.Storage;
using SmartSolutionsLab.BauDoku.Documentation.Application.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Storage;

public sealed partial class ChunkedUploadCleanupService(IOptions<PhotoStorageOptions> options, ILogger<ChunkedUploadCleanupService> logger)
    : BackgroundService
{
    private const string MetadataFileName = "metadata.json";

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

    private async Task CleanupExpiredSessionsAsync(CancellationToken cancellationToken)
    {
        var sessionDirs = storage.GetSubdirectories();
        var cleaned = 0;

        foreach (var sessionDir in sessionDirs)
        {
            var sessionName = Path.GetFileName(sessionDir);
            var metadataPath = Path.Combine(sessionName, MetadataFileName);

            if (!storage.FileExists(metadataPath))
            {
                TryDeleteDirectory(sessionName);
                cleaned++;
                continue;
            }

            try
            {
                var session = await storage.ReadJsonAsync<ChunkedUploadSession>(metadataPath, cancellationToken);
                if (session is null || DateTime.UtcNow - session.CreatedAt > maxAge)
                {
                    TryDeleteDirectory(sessionName);
                    cleaned++;
                }
            }
            catch (Exception ex)
            {
                LogSessionMetadataReadError(ex, sessionDir);
                TryDeleteDirectory(sessionName);
                cleaned++;
            }
        }

        if (cleaned > 0)
        {
            LogCleanupCompleted(cleaned);
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
            LogDirectoryDeleteError(ex, relativePath);
        }
    }

    [LoggerMessage(EventId = 2001, Level = LogLevel.Warning,
        Message = "Fehler beim Lesen der Session-Metadaten in {SessionDir}")]
    private partial void LogSessionMetadataReadError(Exception exception, string sessionDir);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Information,
        Message = "Chunked-Upload-Cleanup: {Count} abgelaufene Sessions entfernt")]
    private partial void LogCleanupCompleted(int count);

    [LoggerMessage(EventId = 2003, Level = LogLevel.Warning,
        Message = "Fehler beim Löschen des Session-Verzeichnisses {Path}")]
    private partial void LogDirectoryDeleteError(Exception exception, string path);
}
