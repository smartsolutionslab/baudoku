using BauDoku.Documentation.Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalFilePhotoStorage : IPhotoStorage
{
    private readonly string _basePath;

    public LocalFilePhotoStorage(IConfiguration configuration)
    {
        _basePath = configuration["PhotoStorage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads", "photos");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(_basePath, uniqueName);

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream, ct);

        return uniqueName;
    }

    public Task<Stream> DownloadAsync(string blobUrl, CancellationToken ct = default)
    {
        var filePath = Path.Combine(_basePath, blobUrl);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Foto nicht gefunden: {blobUrl}");

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string blobUrl, CancellationToken ct = default)
    {
        var filePath = Path.Combine(_basePath, blobUrl);
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
