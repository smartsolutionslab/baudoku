using BauDoku.BuildingBlocks.Storage;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalFilePhotoStorage(IOptions<PhotoStorageOptions> options) : IPhotoStorage
{
    private readonly LocalStorageDirectory storage = new(options.Value.LocalPath);

    public async Task<BlobUrl> UploadAsync(Stream stream, FileName fileName, ContentType contentType, CancellationToken cancellationToken = default)
    {
        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName.Value)}";
        await storage.WriteStreamAsync(uniqueName, stream, cancellationToken);
        return BlobUrl.From(uniqueName);
    }

    public Task<Stream> DownloadAsync(BlobUrl blobUrl, CancellationToken cancellationToken = default)
    {
        storage.EnsureFileExists(blobUrl.Value);
        Stream stream = storage.OpenRead(blobUrl.Value);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(BlobUrl blobUrl, CancellationToken cancellationToken = default)
    {
        storage.DeleteFile(blobUrl.Value);
        return Task.CompletedTask;
    }
}
