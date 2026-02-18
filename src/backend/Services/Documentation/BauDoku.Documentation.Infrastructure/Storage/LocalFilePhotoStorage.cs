using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalFilePhotoStorage : IPhotoStorage
{
    private readonly string basePath;

    public LocalFilePhotoStorage(IOptions<PhotoStorageOptions> options)
    {
        basePath = options.Value.LocalPath;
        if (!Path.IsPathRooted(basePath)) basePath = Path.Combine(Directory.GetCurrentDirectory(), basePath);
        Directory.CreateDirectory(basePath);
    }

    public async Task<BlobUrl> UploadAsync(Stream stream, FileName fileName, ContentType contentType, CancellationToken ct = default)
    {
        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName.Value)}";
        var filePath = Path.Combine(basePath, uniqueName);

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream, ct);

        return BlobUrl.From(uniqueName);
    }

    public Task<Stream> DownloadAsync(BlobUrl blobUrl, CancellationToken ct = default)
    {
        var filePath = SafeResolvePath(blobUrl.Value);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"Foto nicht gefunden: {blobUrl.Value}");

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(BlobUrl blobUrl, CancellationToken ct = default)
    {
        var filePath = SafeResolvePath(blobUrl.Value);
        if (File.Exists(filePath)) File.Delete(filePath);

        return Task.CompletedTask;
    }

    private string SafeResolvePath(string blobUrl)
    {
        var filePath = Path.Combine(basePath, blobUrl);
        var fullPath = Path.GetFullPath(filePath);
        return !fullPath.StartsWith(Path.GetFullPath(basePath), StringComparison.OrdinalIgnoreCase) ? throw new UnauthorizedAccessException($"Zugriff verweigert: {blobUrl}") : fullPath;
    }
}
