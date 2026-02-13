using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;

public sealed class CompleteChunkedUploadCommandHandler : ICommandHandler<CompleteChunkedUploadCommand, Guid>
{
    private readonly IChunkedUploadStorage _chunkedUploadStorage;
    private readonly IPhotoStorage _photoStorage;
    private readonly IInstallationRepository _installationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteChunkedUploadCommandHandler(
        IChunkedUploadStorage chunkedUploadStorage,
        IPhotoStorage photoStorage,
        IInstallationRepository installationRepository,
        IUnitOfWork unitOfWork)
    {
        _chunkedUploadStorage = chunkedUploadStorage;
        _photoStorage = photoStorage;
        _installationRepository = installationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CompleteChunkedUploadCommand command, CancellationToken cancellationToken)
    {
        var session = await _chunkedUploadStorage.GetSessionAsync(command.SessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Upload-Session mit ID {command.SessionId} nicht gefunden.");

        var uploadedChunks = await _chunkedUploadStorage.GetUploadedChunkCountAsync(command.SessionId, cancellationToken);
        if (uploadedChunks != session.TotalChunks)
            throw new InvalidOperationException(
                $"Upload unvollständig: {uploadedChunks}/{session.TotalChunks} Chunks hochgeladen.");

        await using var assembledStream = await _chunkedUploadStorage.AssembleAsync(command.SessionId, cancellationToken);

        var blobUrl = await _photoStorage.UploadAsync(
            assembledStream, session.FileName, session.ContentType, cancellationToken);

        var installationId = new InstallationId(session.InstallationId);
        var installation = await _installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {session.InstallationId} nicht gefunden.");

        var photoId = PhotoId.New();
        var photoType = new PhotoType(session.PhotoType);
        var caption = session.Caption is not null ? new Caption(session.Caption) : null;
        var description = session.Description is not null ? new Description(session.Description) : null;

        GpsPosition? position = null;
        if (session.Latitude.HasValue && session.Longitude.HasValue
            && session.HorizontalAccuracy.HasValue && session.GpsSource is not null)
        {
            position = new GpsPosition(
                session.Latitude.Value,
                session.Longitude.Value,
                session.Altitude,
                session.HorizontalAccuracy.Value,
                session.GpsSource);
        }

        installation.AddPhoto(
            photoId, session.FileName, blobUrl, session.ContentType, session.TotalSize,
            photoType, caption, description, position);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(session.TotalSize);

        await _chunkedUploadStorage.CleanupSessionAsync(command.SessionId, cancellationToken);

        return photoId.Value;
    }
}
