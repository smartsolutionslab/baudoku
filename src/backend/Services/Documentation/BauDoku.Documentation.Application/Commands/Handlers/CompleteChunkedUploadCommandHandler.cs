using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Diagnostics;
using BauDoku.Documentation.ReadModel;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class CompleteChunkedUploadCommandHandler(IChunkedUploadStorage chunkedUploadStorage, IPhotoStorage photoStorage, IInstallationRepository installations)
    : ICommandHandler<CompleteChunkedUploadCommand, PhotoIdentifier>
{
    public async Task<PhotoIdentifier> Handle(CompleteChunkedUploadCommand command, CancellationToken cancellationToken = default)
    {
        var sessionId = command.SessionId;

        var session = await chunkedUploadStorage.GetSessionAsync(sessionId, cancellationToken);

        var uploadedChunks = await chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, cancellationToken);
        if (uploadedChunks != session.TotalChunks) throw new InvalidOperationException($"Upload unvollständig: {uploadedChunks}/{session.TotalChunks} Chunks hochgeladen.");

        await using var assembledStream = await chunkedUploadStorage.AssembleAsync(sessionId, cancellationToken);

        var fileNameVo = FileName.From(session.FileName);
        var contentTypeVo = ContentType.From(session.ContentType);
        var blobUrl = await photoStorage.UploadAsync(assembledStream, fileNameVo, contentTypeVo, cancellationToken);

        var installationId = InstallationIdentifier.From(session.InstallationId);
        var installation = await installations.With(installationId, cancellationToken);

        var photoId = PhotoIdentifier.New();
        var photoType = PhotoType.From(session.PhotoType);
        var caption = Caption.FromNullable(session.Caption);
        var description = Description.FromNullable(session.Description);

        var position = session.Position?.ToDomain();

        installation.AddPhoto(photoId, fileNameVo, blobUrl, contentTypeVo, FileSize.From(session.TotalSize), photoType, caption, description, position);

        await installations.SaveAsync(installation, cancellationToken);

        DocumentationMetrics.PhotosAdded.Add(1);
        DocumentationMetrics.PhotoFileSize.Record(session.TotalSize);

        await chunkedUploadStorage.CleanupSessionAsync(sessionId, cancellationToken);

        return photoId;
    }
}
