using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands;

public sealed record UploadChunkCommand(
    UploadSessionIdentifier SessionId,
    ChunkIndex ChunkIndex,
    Stream Data) : ICommand;
