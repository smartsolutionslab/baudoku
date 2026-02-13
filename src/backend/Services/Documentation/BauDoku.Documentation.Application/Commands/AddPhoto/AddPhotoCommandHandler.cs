using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.AddPhoto;

public sealed class AddPhotoCommandHandler : ICommandHandler<AddPhotoCommand, Guid>
{
    private readonly IInstallationRepository _installationRepository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IUnitOfWork _unitOfWork;

    public AddPhotoCommandHandler(
        IInstallationRepository installationRepository,
        IPhotoStorage photoStorage,
        IUnitOfWork unitOfWork)
    {
        _installationRepository = installationRepository;
        _photoStorage = photoStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddPhotoCommand command, CancellationToken cancellationToken)
    {
        var installationId = new InstallationId(command.InstallationId);
        var installation = await _installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var blobUrl = await _photoStorage.UploadAsync(
            command.Stream, command.FileName, command.ContentType, cancellationToken);

        var photoId = PhotoId.New();
        var photoType = new PhotoType(command.PhotoType);
        var caption = command.Caption is not null ? new Caption(command.Caption) : null;
        var description = command.Description is not null ? new Description(command.Description) : null;

        GpsPosition? position = null;
        if (command.Latitude.HasValue && command.Longitude.HasValue
            && command.HorizontalAccuracy.HasValue && command.GpsSource is not null)
        {
            position = new GpsPosition(
                command.Latitude.Value,
                command.Longitude.Value,
                command.Altitude,
                command.HorizontalAccuracy.Value,
                command.GpsSource);
        }

        installation.AddPhoto(
            photoId, command.FileName, blobUrl, command.ContentType, command.FileSize,
            photoType, caption, description, position);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(command.FileSize);

        return photoId.Value;
    }
}
