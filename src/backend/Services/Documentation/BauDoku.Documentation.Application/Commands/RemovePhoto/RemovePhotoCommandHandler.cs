using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RemovePhoto;

public sealed class RemovePhotoCommandHandler(IInstallationRepository installations, IPhotoStorage photoStorage, IUnitOfWork unitOfWork)
    : ICommandHandler<RemovePhotoCommand>
{
    public async Task Handle(RemovePhotoCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, photoId) = command;
        var installationIdentifier =  InstallationIdentifier.From(installationId);
        var installation = await installations.GetByIdAsync(installationIdentifier, cancellationToken);

        var photoIdentifier = PhotoIdentifier.From(photoId);
        var photo = installation.Photos.FirstOrDefault(p => p.Id == photoIdentifier)  ?? throw new KeyNotFoundException($"Foto mit ID {photoId} nicht gefunden.");

        installation.RemovePhoto(photoIdentifier);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await photoStorage.DeleteAsync(photo.BlobUrl, cancellationToken);

        DocumentationMetrics.PhotosRemoved.Add(1);
    }
}
