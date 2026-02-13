using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BauDoku.Documentation.Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class AzureBlobPhotoStorage : IPhotoStorage
{
    private readonly BlobContainerClient containerClient;

    public AzureBlobPhotoStorage(IConfiguration configuration)
    {
        var connectionString = configuration["PhotoStorage:AzureConnectionString"]
            ?? throw new InvalidOperationException("PhotoStorage:AzureConnectionString is not configured.");
        var containerName = configuration["PhotoStorage:AzureContainerName"] ?? "photos";

        containerClient = new BlobContainerClient(connectionString, containerName);
        containerClient.CreateIfNotExists();
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, ct);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadAsync(string blobUrl, CancellationToken ct = default)
    {
        var blobName = new Uri(blobUrl).Segments[^1];
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
        return response.Value.Content;
    }

    public async Task DeleteAsync(string blobUrl, CancellationToken ct = default)
    {
        var blobName = new Uri(blobUrl).Segments[^1];
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }
}
