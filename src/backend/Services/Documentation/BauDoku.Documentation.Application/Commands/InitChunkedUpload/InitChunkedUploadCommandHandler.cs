using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.InitChunkedUpload;

public sealed class InitChunkedUploadCommandHandler(IInstallationRepository installations, IChunkedUploadStorage chunkedUploadStorage)
    : ICommandHandler<InitChunkedUploadCommand, Guid>
{
    public async Task<Guid> Handle(InitChunkedUploadCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, fileName, contentType, totalSize, totalChunks, photoType,
             caption, description, latitude, longitude, altitude, horizontalAccuracy, gpsSource) = command;

        _ = await installations.GetByIdAsync(InstallationIdentifier.From(installationId), cancellationToken);

        var session = new ChunkedUploadSession(
            SessionId: Guid.NewGuid(),
            InstallationId: installationId,
            FileName: fileName,
            ContentType: contentType,
            TotalSize: totalSize,
            TotalChunks: totalChunks,
            PhotoType: photoType,
            Caption: caption,
            Description: description,
            Latitude: latitude,
            Longitude: longitude,
            Altitude: altitude,
            HorizontalAccuracy: horizontalAccuracy,
            GpsSource: gpsSource,
            CreatedAt: DateTime.UtcNow);

        var sessionId = await chunkedUploadStorage.InitSessionAsync(session, cancellationToken);
        return sessionId;
    }
}
