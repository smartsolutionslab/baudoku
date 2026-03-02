using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class InitChunkedUploadCommandHandler(IInstallationRepository installations, IChunkedUploadStorage chunkedUploadStorage)
    : ICommandHandler<InitChunkedUploadCommand, UploadSessionIdentifier>
{
    public async Task<UploadSessionIdentifier> Handle(InitChunkedUploadCommand command, CancellationToken cancellationToken = default)
    {
        _ = await installations.GetByIdAsync(command.InstallationId, cancellationToken);

        var sessionIdentifier = UploadSessionIdentifier.New();
        var session = new ChunkedUploadSession(
            SessionId: sessionIdentifier.Value,
            InstallationId: command.InstallationId.Value,
            FileName: command.FileName.Value,
            ContentType: command.ContentType.Value,
            TotalSize: command.TotalSize.Value,
            TotalChunks: command.TotalChunks.Value,
            PhotoType: command.PhotoType.Value,
            Caption: command.Caption?.Value,
            Description: command.Description?.Value,
            Position: command.Position?.ToGpsDto(),
            CreatedAt: DateTime.UtcNow);

        await chunkedUploadStorage.InitSessionAsync(session, cancellationToken);
        return sessionIdentifier;
    }
}
