using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Application.Contracts;
using SmartSolutionsLab.BauDoku.Documentation.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

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
