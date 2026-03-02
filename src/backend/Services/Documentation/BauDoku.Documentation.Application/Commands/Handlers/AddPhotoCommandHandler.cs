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
        var installation = await installations.GetByIdAsync(command.InstallationId, cancellationToken);

        var blobUrl = await photoStorage.UploadAsync(command.Stream, command.FileName, command.ContentType, cancellationToken);

        var photoId = PhotoIdentifier.New();

        installation.AddPhoto(
            photoId, command.FileName, blobUrl, command.ContentType, command.FileSize,
            command.PhotoType, command.Caption, command.Description, command.Position, command.TakenAt);

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
        DocumentationMetrics.PhotoFileSize.Record(command.FileSize.Value);

        return photoId;
    }
}
