using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Application.Contracts;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;

public sealed class InitChunkedUploadCommandHandler(IInstallationRepository installations, IChunkedUploadStorage chunkedUploadStorage)
    : ICommandHandler<InitChunkedUploadCommand, UploadSessionIdentifier>
{
    public async Task<UploadSessionIdentifier> Handle(InitChunkedUploadCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, fileName, contentType, totalSize, totalChunks, photoType, caption, description, position) = command;

        _ = await installations.With(installationId, cancellationToken);

        var sessionIdentifier = UploadSessionIdentifier.New();
        var session = new ChunkedUploadSession(
            SessionId: sessionIdentifier.Value,
            InstallationId: installationId.Value,
            FileName: fileName.Value,
            ContentType: contentType.Value,
            TotalSize: totalSize.Value,
            TotalChunks: totalChunks.Value,
            PhotoType: photoType.Value,
            Caption: caption?.Value,
            Description: description?.Value,
            Position: position?.ToGpsDto(),
            CreatedAt: DateTime.UtcNow);

        await chunkedUploadStorage.InitSessionAsync(session, cancellationToken);
        return sessionIdentifier;
    }
}
