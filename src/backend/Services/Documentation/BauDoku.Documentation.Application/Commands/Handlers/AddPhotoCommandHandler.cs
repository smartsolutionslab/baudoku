using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class AddPhotoCommandHandler(IInstallationRepository installations, IPhotoStorage photoStorage)
    : ICommandHandler<AddPhotoCommand, PhotoIdentifier>
{
    public async Task<PhotoIdentifier> Handle(AddPhotoCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, fileName, contentType, fileSize,
            photoType, caption, description,
            latitude, longitude, altitude,
            horizontalAccuracy, gpsSource,
            stream, takenAt) = command;

        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        var blobUrl = await photoStorage.UploadAsync(stream, fileName, contentType, cancellationToken);

        var photoId = PhotoIdentifier.New();

        GpsPosition? position = null;
        if (latitude is not null && longitude is not null && horizontalAccuracy is not null && gpsSource is not null)
        {
            position = GpsPosition.Create(latitude, longitude, altitude, horizontalAccuracy, gpsSource);
        }

        installation.AddPhoto(
            photoId, fileName, blobUrl, contentType, fileSize,
            photoType, caption, description, position, takenAt);

        try
        {
            await installations.SaveAsync(installation, cancellationToken);
        }
        catch
        {
            await photoStorage.DeleteAsync(blobUrl, cancellationToken);
            throw;
        }

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(fileSize.Value);

        return photoId;
    }
}
