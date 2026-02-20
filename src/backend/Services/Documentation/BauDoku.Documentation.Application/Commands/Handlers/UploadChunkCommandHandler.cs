using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class UploadChunkCommandHandler(IChunkedUploadStorage chunkedUploadStorage) : ICommandHandler<UploadChunkCommand>
{
    public async Task Handle(UploadChunkCommand command, CancellationToken cancellationToken = default)
    {
        var (sessionId, chunkIndex, data) = command;

        var session = await chunkedUploadStorage.GetSessionAsync(sessionId, cancellationToken);

        if (chunkIndex >= session.TotalChunks) throw new InvalidOperationException($"ChunkIndex {chunkIndex} ist ungültig. Erwartet: 0-{session.TotalChunks - 1}.");

        await chunkedUploadStorage.StoreChunkAsync(sessionId, chunkIndex, data, cancellationToken);
    }
}
