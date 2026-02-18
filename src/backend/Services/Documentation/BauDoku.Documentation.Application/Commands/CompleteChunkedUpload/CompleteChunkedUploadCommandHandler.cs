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
        var session = await chunkedUploadStorage.GetSessionAsync(command.SessionId, cancellationToken) ?? throw new KeyNotFoundException($"Upload-Session mit ID {command.SessionId} nicht gefunden.");

        var uploadedChunks = await chunkedUploadStorage.GetUploadedChunkCountAsync(command.SessionId, cancellationToken);
        if (uploadedChunks != session.TotalChunks) throw new InvalidOperationException($"Upload unvollständig: {uploadedChunks}/{session.TotalChunks} Chunks hochgeladen.");

        await using var assembledStream = await chunkedUploadStorage.AssembleAsync(command.SessionId, cancellationToken);

        var fileNameVo = FileName.From(session.FileName);
        var contentTypeVo = ContentType.From(session.ContentType);
        var blobUrl = await photoStorage.UploadAsync(assembledStream, fileNameVo, contentTypeVo, cancellationToken);

        var installationId = InstallationIdentifier.From(session.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        var photoId = PhotoIdentifier.New();
        var photoType = PhotoType.From(session.PhotoType);
        var caption = session.Caption is not null ? Caption.From(session.Caption) : null;
        var description = session.Description is not null ? Description.From(session.Description) : null;

        GpsPosition? position = null;
        if (session.Latitude.HasValue && session.Longitude.HasValue && session.HorizontalAccuracy.HasValue && session.GpsSource is not null)
        {
            position = GpsPosition.Create(
                session.Latitude.Value,
                session.Longitude.Value,
                session.Altitude,
                session.HorizontalAccuracy.Value,
                session.GpsSource);
        }

        installation.AddPhoto(photoId, fileNameVo, blobUrl, contentTypeVo, FileSize.From(session.TotalSize), photoType, caption, description, position);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(session.TotalSize);

        await chunkedUploadStorage.CleanupSessionAsync(command.SessionId, cancellationToken);

        return photoId.Value;
    }
}
