using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands;

public sealed record InitChunkedUploadCommand(
    InstallationIdentifier InstallationId,
    FileName FileName,
    ContentType ContentType,
    FileSize TotalSize,
    ChunkCount TotalChunks,
    PhotoType PhotoType,
    Caption? Caption,
    Description? Description,
    GpsPosition? Position) : ICommand<UploadSessionIdentifier>;
