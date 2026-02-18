using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Contracts;

public interface IPhotoStorage
{
    Task<BlobUrl> UploadAsync(Stream stream, FileName fileName, ContentType contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(BlobUrl blobUrl, CancellationToken ct = default);
    Task DeleteAsync(BlobUrl blobUrl, CancellationToken ct = default);
}
