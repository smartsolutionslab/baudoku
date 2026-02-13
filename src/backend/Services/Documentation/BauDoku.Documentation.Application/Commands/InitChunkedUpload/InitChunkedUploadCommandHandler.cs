using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.InitChunkedUpload;

public sealed class InitChunkedUploadCommandHandler : ICommandHandler<InitChunkedUploadCommand, Guid>
{
    private readonly IInstallationRepository _installationRepository;
    private readonly IChunkedUploadStorage _chunkedUploadStorage;

    public InitChunkedUploadCommandHandler(
        IInstallationRepository installationRepository,
        IChunkedUploadStorage chunkedUploadStorage)
    {
        _installationRepository = installationRepository;
        _chunkedUploadStorage = chunkedUploadStorage;
    }

    public async Task<Guid> Handle(InitChunkedUploadCommand command, CancellationToken cancellationToken)
    {
        var installationId = new InstallationId(command.InstallationId);
        _ = await _installationRepository.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {command.InstallationId} nicht gefunden.");

        var session = new ChunkedUploadSession(
            SessionId: Guid.NewGuid(),
            InstallationId: command.InstallationId,
            FileName: command.FileName,
            ContentType: command.ContentType,
            TotalSize: command.TotalSize,
            TotalChunks: command.TotalChunks,
            PhotoType: command.PhotoType,
            Caption: command.Caption,
            Description: command.Description,
            Latitude: command.Latitude,
            Longitude: command.Longitude,
            Altitude: command.Altitude,
            HorizontalAccuracy: command.HorizontalAccuracy,
            GpsSource: command.GpsSource,
            CreatedAt: DateTime.UtcNow);

        var sessionId = await _chunkedUploadStorage.InitSessionAsync(session, cancellationToken);
        return sessionId;
    }
}
