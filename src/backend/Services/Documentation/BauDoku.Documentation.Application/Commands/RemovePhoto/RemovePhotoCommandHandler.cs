using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RemovePhoto;

public sealed class RemovePhotoCommandHandler(IInstallationRepository installations, IPhotoStorage photoStorage, IUnitOfWork unitOfWork)
    : ICommandHandler<RemovePhotoCommand>
{
    public async Task Handle(RemovePhotoCommand command, CancellationToken cancellationToken)
    {
        var installationId = InstallationIdentifier.From(command.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken) ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var photoId = PhotoIdentifier.From(command.PhotoId);
        var photo = installation.Photos.FirstOrDefault(p => p.Id == photoId) ?? throw new InvalidOperationException($"Foto mit ID {command.PhotoId} nicht gefunden.");

        installation.RemovePhoto(photoId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await photoStorage.DeleteAsync(photo.BlobUrl, cancellationToken);

        DocumentationMetrics.PhotosRemoved.Add(1);
    }
}
