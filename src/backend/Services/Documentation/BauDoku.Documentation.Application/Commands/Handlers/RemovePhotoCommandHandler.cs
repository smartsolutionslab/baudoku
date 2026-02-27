using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class RemovePhotoCommandHandler(IInstallationRepository installations, IPhotoStorage photoStorage)
    : ICommandHandler<RemovePhotoCommand>
{
    public async Task Handle(RemovePhotoCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, photoId) = command;

        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        var photo = installation.Photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new KeyNotFoundException($"Foto mit ID {photoId.Value} nicht gefunden.");

        installation.RemovePhoto(photoId);

        await installations.SaveAsync(installation, cancellationToken);

        await photoStorage.DeleteAsync(photo.BlobUrl, cancellationToken);

        DocumentationMetrics.PhotosRemoved.Add(1);
    }
}
