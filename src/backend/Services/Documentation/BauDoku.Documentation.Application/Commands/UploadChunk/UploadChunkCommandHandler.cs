using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Application.Contracts;

namespace BauDoku.Documentation.Application.Commands.UploadChunk;

public sealed class UploadChunkCommandHandler : ICommandHandler<UploadChunkCommand>
{
    private readonly IChunkedUploadStorage _chunkedUploadStorage;

    public UploadChunkCommandHandler(IChunkedUploadStorage chunkedUploadStorage)
    {
        _chunkedUploadStorage = chunkedUploadStorage;
    }

    public async Task Handle(UploadChunkCommand command, CancellationToken cancellationToken)
    {
        var session = await _chunkedUploadStorage.GetSessionAsync(command.SessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Upload-Session mit ID {command.SessionId} nicht gefunden.");

        if (command.ChunkIndex >= session.TotalChunks)
            throw new InvalidOperationException(
                $"ChunkIndex {command.ChunkIndex} ist ungültig. Erwartet: 0-{session.TotalChunks - 1}.");

        await _chunkedUploadStorage.StoreChunkAsync(command.SessionId, command.ChunkIndex, command.Data, cancellationToken);
    }
}
