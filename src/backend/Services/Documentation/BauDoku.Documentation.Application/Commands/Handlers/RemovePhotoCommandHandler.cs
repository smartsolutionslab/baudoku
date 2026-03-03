using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Application.Contracts;
using SmartSolutionsLab.BauDoku.Documentation.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

public sealed class RemovePhotoCommandHandler(IInstallationRepository installations, IPhotoStorage photoStorage)
    : ICommandHandler<RemovePhotoCommand>
{
    public async Task Handle(RemovePhotoCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, photoId) = command;

        var installation = await installations.With(installationId, cancellationToken);
        var photo = installation.Photos.FirstOrDefault(p => p.Id == photoId) ?? throw new KeyNotFoundException($"Foto mit ID {photoId.Value} nicht gefunden.");
        installation.RemovePhoto(photoId);
        await installations.SaveAsync(installation, cancellationToken);
        await photoStorage.DeleteAsync(photo.BlobUrl, cancellationToken);

        DocumentationMetrics.PhotosRemoved.Add(1);
    }
}
