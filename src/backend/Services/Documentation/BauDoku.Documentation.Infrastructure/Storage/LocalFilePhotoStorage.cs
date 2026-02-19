using BauDoku.BuildingBlocks.Infrastructure.Storage;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class LocalFilePhotoStorage : IPhotoStorage
{
    private readonly LocalStorageDirectory storage;

    public LocalFilePhotoStorage(IOptions<PhotoStorageOptions> options)
    {
        storage = new LocalStorageDirectory(options.Value.LocalPath);
    }

    public async Task<BlobUrl> UploadAsync(Stream stream, FileName fileName, ContentType contentType, CancellationToken ct = default)
    {
        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName.Value)}";
        await storage.WriteStreamAsync(uniqueName, stream, ct);
        return BlobUrl.From(uniqueName);
    }

    public Task<Stream> DownloadAsync(BlobUrl blobUrl, CancellationToken ct = default)
    {
        if (!storage.FileExists(blobUrl.Value))
            throw new FileNotFoundException($"Foto nicht gefunden: {blobUrl.Value}");

        Stream stream = storage.OpenRead(blobUrl.Value);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(BlobUrl blobUrl, CancellationToken ct = default)
    {
        if (storage.FileExists(blobUrl.Value))
            storage.DeleteFile(blobUrl.Value);

        return Task.CompletedTask;
    }
}
