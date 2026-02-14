using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RemovePhoto;

public sealed class RemovePhotoCommandHandler : ICommandHandler<RemovePhotoCommand>
{
    private readonly IInstallationRepository installationRepository;
    private readonly IPhotoStorage photoStorage;
    private readonly IUnitOfWork unitOfWork;

    public RemovePhotoCommandHandler(
        IInstallationRepository installationRepository,
        IPhotoStorage photoStorage,
        IUnitOfWork unitOfWork)
    {
        this.installationRepository = installationRepository;
        this.photoStorage = photoStorage;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(RemovePhotoCommand command, CancellationToken cancellationToken)
    {
        var installationId = InstallationIdentifier.From(command.InstallationId);
        var installation = await installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var photoId = PhotoIdentifier.From(command.PhotoId);
        var photo = installation.Photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new InvalidOperationException($"Foto mit ID {command.PhotoId} nicht gefunden.");

        installation.RemovePhoto(photoId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await photoStorage.DeleteAsync(photo.BlobUrl, cancellationToken);

        DocumentationMetrics.PhotosRemoved.Add(1);
    }
}
