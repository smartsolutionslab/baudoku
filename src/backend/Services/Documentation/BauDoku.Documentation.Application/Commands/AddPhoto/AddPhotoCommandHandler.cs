using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.AddPhoto;

public sealed class AddPhotoCommandHandler : ICommandHandler<AddPhotoCommand, Guid>
{
    private readonly IInstallationRepository installationRepository;
    private readonly IPhotoStorage photoStorage;
    private readonly IUnitOfWork unitOfWork;

    public AddPhotoCommandHandler(
        IInstallationRepository installationRepository,
        IPhotoStorage photoStorage,
        IUnitOfWork unitOfWork)
    {
        this.installationRepository = installationRepository;
        this.photoStorage = photoStorage;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddPhotoCommand command, CancellationToken cancellationToken)
    {
        var installationId = InstallationIdentifier.From(command.InstallationId);
        var installation = await installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var blobUrl = await photoStorage.UploadAsync(
            command.Stream, command.FileName, command.ContentType, cancellationToken);

        var photoId = PhotoIdentifier.New();
        var photoType = PhotoType.From(command.PhotoType);
        var caption = command.Caption is not null ? Caption.From(command.Caption) : null;
        var description = command.Description is not null ? Description.From(command.Description) : null;

        GpsPosition? position = null;
        if (command.Latitude.HasValue && command.Longitude.HasValue
            && command.HorizontalAccuracy.HasValue && command.GpsSource is not null)
        {
            position = GpsPosition.Create(
                command.Latitude.Value,
                command.Longitude.Value,
                command.Altitude,
                command.HorizontalAccuracy.Value,
                command.GpsSource);
        }

        installation.AddPhoto(
            photoId, command.FileName, blobUrl, command.ContentType, command.FileSize,
            photoType, caption, description, position);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(command.FileSize);

        return photoId.Value;
    }
}
