using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.RemovePhoto;

public sealed class RemovePhotoCommandHandler : ICommandHandler<RemovePhotoCommand>
{
    private readonly IInstallationRepository _installationRepository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePhotoCommandHandler(
        IInstallationRepository installationRepository,
        IPhotoStorage photoStorage,
        IUnitOfWork unitOfWork)
    {
        _installationRepository = installationRepository;
        _photoStorage = photoStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemovePhotoCommand command, CancellationToken cancellationToken)
    {
        var installationId = new InstallationId(command.InstallationId);
        var installation = await _installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var photoId = new PhotoId(command.PhotoId);
        var photo = installation.Photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new InvalidOperationException($"Foto mit ID {command.PhotoId} nicht gefunden.");

        await _photoStorage.DeleteAsync(photo.BlobUrl, cancellationToken);

        installation.RemovePhoto(photoId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.PhotosRemoved.Add(1);
    }
}
