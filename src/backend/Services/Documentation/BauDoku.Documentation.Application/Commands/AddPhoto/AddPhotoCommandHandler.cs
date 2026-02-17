using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.AddPhoto;

public sealed class AddPhotoCommandHandler(IInstallationRepository installations, IPhotoStorage photoStorage, IUnitOfWork unitOfWork)
    : ICommandHandler<AddPhotoCommand, Guid>
{
    public async Task<Guid> Handle(AddPhotoCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, fileName, contentType, fileSize, photoTypeName, captionText, descriptionText,
             latitude, longitude, altitude, horizontalAccuracy, gpsSource, stream, takenAt) = command;

        var installation = await installations.GetByIdAsync(
            InstallationIdentifier.From(installationId), cancellationToken)
            ?? throw new KeyNotFoundException($"Installation mit ID {installationId} nicht gefunden.");

        var blobUrl = await photoStorage.UploadAsync(stream, fileName, contentType, cancellationToken);

        var photoId = PhotoIdentifier.New();
        var photoType = PhotoType.From(photoTypeName);
        var caption = captionText is not null ? Caption.From(captionText) : null;
        var description = descriptionText is not null ? Description.From(descriptionText) : null;

        GpsPosition? position = null;
        if (latitude.HasValue && longitude.HasValue
            && horizontalAccuracy.HasValue && gpsSource is not null)
        {
            position = GpsPosition.Create(
                latitude.Value, longitude.Value, altitude, horizontalAccuracy.Value, gpsSource);
        }

        installation.AddPhoto(
            photoId, fileName, blobUrl, contentType, fileSize,
            photoType, caption, description, position, takenAt);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await photoStorage.DeleteAsync(blobUrl, cancellationToken);
            throw;
        }

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(fileSize);

        return photoId.Value;
    }
}
