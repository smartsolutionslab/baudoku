using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Commands.UploadChunk;

public sealed class UploadChunkCommandHandler(IChunkedUploadStorage chunkedUploadStorage) : ICommandHandler<UploadChunkCommand>
{
    public async Task Handle(UploadChunkCommand command, CancellationToken cancellationToken = default)
    {
        var (sessionId, chunkIndex, data) = command;
        var sessionIdentifier = UploadSessionIdentifier.From(sessionId);

        var session = await chunkedUploadStorage.GetSessionAsync(sessionIdentifier, cancellationToken) ?? throw new KeyNotFoundException($"Upload-Session mit ID {sessionId} nicht gefunden.");

        if (chunkIndex >= session.TotalChunks) throw new InvalidOperationException($"ChunkIndex {chunkIndex} ist ungültig. Erwartet: 0-{session.TotalChunks - 1}.");

        await chunkedUploadStorage.StoreChunkAsync(sessionIdentifier, chunkIndex, data, cancellationToken);
    }
}
