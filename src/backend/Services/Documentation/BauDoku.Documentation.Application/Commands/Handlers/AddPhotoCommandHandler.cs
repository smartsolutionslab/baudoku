using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class AddPhotoCommandHandler(IInstallationRepository installations, IPhotoStorage photoStorage)
    : ICommandHandler<AddPhotoCommand, PhotoIdentifier>
{
    public async Task<PhotoIdentifier> Handle(AddPhotoCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, fileName, contentType, fileSize, photoType, caption, description, position, stream, takenAt) = command;

        var installation = await installations.With(installationId, cancellationToken);
        var blobUrl = await photoStorage.UploadAsync(stream, fileName, contentType, cancellationToken);
        var photoId = PhotoIdentifier.New();

        installation.AddPhoto(photoId, fileName, blobUrl, contentType, fileSize, photoType, caption, description, position, takenAt);

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
