using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.UploadChunk;

public sealed record UploadChunkCommand(
    Guid SessionId,
    int ChunkIndex,
    Stream Data) : ICommand;
