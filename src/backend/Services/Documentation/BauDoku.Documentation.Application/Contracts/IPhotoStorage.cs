using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Contracts;

public interface IPhotoStorage
{
    Task<BlobUrl> UploadAsync(Stream stream, FileName fileName, ContentType contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(BlobUrl blobUrl, CancellationToken cancellationToken = default);
    Task DeleteAsync(BlobUrl blobUrl, CancellationToken cancellationToken = default);
}
