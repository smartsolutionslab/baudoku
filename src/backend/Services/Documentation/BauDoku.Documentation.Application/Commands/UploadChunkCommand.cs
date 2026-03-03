using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands;

public sealed record UploadChunkCommand(
    UploadSessionIdentifier SessionId,
    ChunkIndex ChunkIndex,
    Stream Data) : ICommand;
