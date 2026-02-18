using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;

public sealed class CompleteChunkedUploadCommandHandler(IChunkedUploadStorage chunkedUploadStorage, IPhotoStorage photoStorage, IInstallationRepository installations, IUnitOfWork unitOfWork)
    : ICommandHandler<CompleteChunkedUploadCommand, Guid>
{
    public async Task<Guid> Handle(CompleteChunkedUploadCommand command, CancellationToken cancellationToken = default)
    {
        var sessionIdentifier = UploadSessionIdentifier.From(command.SessionId);
        var session = await chunkedUploadStorage.GetSessionAsync(sessionIdentifier, cancellationToken);

        var uploadedChunks = await chunkedUploadStorage.GetUploadedChunkCountAsync(sessionIdentifier, cancellationToken);
        if (uploadedChunks != session.TotalChunks) throw new InvalidOperationException($"Upload unvollständig: {uploadedChunks}/{session.TotalChunks} Chunks hochgeladen.");

        await using var assembledStream = await chunkedUploadStorage.AssembleAsync(sessionIdentifier, cancellationToken);

        var fileNameVo = FileName.From(session.FileName);
        var contentTypeVo = ContentType.From(session.ContentType);
        var blobUrl = await photoStorage.UploadAsync(assembledStream, fileNameVo, contentTypeVo, cancellationToken);

        var installationId = InstallationIdentifier.From(session.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        var photoId = PhotoIdentifier.New();
        var photoType = PhotoType.From(session.PhotoType);
        var caption = Caption.FromNullable(session.Caption);
        var description = Description.FromNullable(session.Description);

        GpsPosition? position = null;
        if (session.Latitude.HasValue && session.Longitude.HasValue && session.HorizontalAccuracy.HasValue && session.GpsSource is not null)
        {
            position = GpsPosition.Create(
                Latitude.From(session.Latitude.Value),
                Longitude.From(session.Longitude.Value),
                session.Altitude,
                HorizontalAccuracy.From(session.HorizontalAccuracy.Value),
                GpsSource.From(session.GpsSource));
        }

        installation.AddPhoto(photoId, fileNameVo, blobUrl, contentTypeVo, FileSize.From(session.TotalSize), photoType, caption, description, position);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(session.TotalSize);

        await chunkedUploadStorage.CleanupSessionAsync(sessionIdentifier, cancellationToken);

        return photoId.Value;
    }
}
