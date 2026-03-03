using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SmartSolutionsLab.BauDoku.Documentation.Application.Contracts;
using SmartSolutionsLab.BauDoku.Documentation.Domain;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Storage;

public sealed class AzureBlobPhotoStorage : IPhotoStorage
{
    private readonly BlobContainerClient containerClient;

    public AzureBlobPhotoStorage(IOptions<PhotoStorageOptions> options)
    {
        var config = options.Value;
        var connectionString = config.AzureConnectionString ?? throw new InvalidOperationException("PhotoStorage:AzureConnectionString is not configured.");

        containerClient = new BlobContainerClient(connectionString, config.AzureContainerName);
        containerClient.CreateIfNotExists();
    }

    public async Task<BlobUrl> UploadAsync(Stream stream, FileName fileName, ContentType contentType, CancellationToken cancellationToken = default)
    {
        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName.Value)}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders { ContentType = contentType.Value };
        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, cancellationToken);

        return BlobUrl.From(blobClient.Uri.ToString());
    }

    public async Task<Stream> DownloadAsync(BlobUrl blobUrl, CancellationToken cancellationToken = default)
    {
        var blobName = new Uri(blobUrl.Value).Segments[^1];
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return response.Value.Content;
    }

    public async Task DeleteAsync(BlobUrl blobUrl, CancellationToken cancellationToken = default)
    {
        var blobName = new Uri(blobUrl.Value).Segments[^1];
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
