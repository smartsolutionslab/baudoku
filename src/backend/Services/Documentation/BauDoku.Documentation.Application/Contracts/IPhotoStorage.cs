namespace BauDoku.Documentation.Application.Contracts;

public interface IPhotoStorage
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string blobUrl, CancellationToken ct = default);
    Task DeleteAsync(string blobUrl, CancellationToken ct = default);
}
