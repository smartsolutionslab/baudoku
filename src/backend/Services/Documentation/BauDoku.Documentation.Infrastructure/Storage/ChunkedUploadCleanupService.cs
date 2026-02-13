using System.Text.Json;
using BauDoku.Documentation.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class ChunkedUploadCleanupService : BackgroundService
{
    private readonly string _basePath;
    private readonly ILogger<ChunkedUploadCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);
    private readonly TimeSpan _maxAge = TimeSpan.FromHours(1);

    public ChunkedUploadCleanupService(
        IConfiguration configuration,
        ILogger<ChunkedUploadCleanupService> logger)
    {
        _basePath = configuration["PhotoStorage:ChunkedPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads", "chunks");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            CleanupExpiredSessions();
        }
    }

    private void CleanupExpiredSessions()
    {
        if (!Directory.Exists(_basePath))
            return;

        var sessionDirs = Directory.GetDirectories(_basePath);
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
                if (session is null || DateTime.UtcNow - session.CreatedAt > _maxAge)
                {
                    TryDeleteDirectory(sessionDir);
                    cleaned++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fehler beim Lesen der Session-Metadaten in {SessionDir}", sessionDir);
                TryDeleteDirectory(sessionDir);
                cleaned++;
            }
        }

        if (cleaned > 0)
            _logger.LogInformation("Chunked-Upload-Cleanup: {Count} abgelaufene Sessions entfernt", cleaned);
    }

    private void TryDeleteDirectory(string path)
    {
        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fehler beim Löschen des Session-Verzeichnisses {Path}", path);
        }
    }
}
