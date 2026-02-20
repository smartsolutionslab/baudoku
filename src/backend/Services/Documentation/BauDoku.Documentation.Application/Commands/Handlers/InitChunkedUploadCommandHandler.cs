using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class InitChunkedUploadCommandHandler(IInstallationRepository installations, IChunkedUploadStorage chunkedUploadStorage)
    : ICommandHandler<InitChunkedUploadCommand, Guid>
{
    public async Task<Guid> Handle(InitChunkedUploadCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, fileName, contentType, totalSize, totalChunks,
            photoType, caption, description,
            latitude, longitude, altitude,
            horizontalAccuracy, gpsSource) = command;

        _ = await installations.GetByIdAsync(installationId, cancellationToken);

        var sessionIdentifier = UploadSessionIdentifier.New();
        var session = new ChunkedUploadSession(
            SessionId: sessionIdentifier.Value,
            InstallationId: installationId.Value,
            FileName: fileName.Value,
            ContentType: contentType.Value,
            TotalSize: totalSize.Value,
            TotalChunks: totalChunks,
            PhotoType: photoType.Value,
            Caption: caption?.Value,
            Description: description?.Value,
            Latitude: latitude?.Value,
            Longitude: longitude?.Value,
            Altitude: altitude,
            HorizontalAccuracy: horizontalAccuracy?.Value,
            GpsSource: gpsSource?.Value,
            CreatedAt: DateTime.UtcNow);

        await chunkedUploadStorage.InitSessionAsync(session, cancellationToken);
        return sessionIdentifier.Value;
    }
}
