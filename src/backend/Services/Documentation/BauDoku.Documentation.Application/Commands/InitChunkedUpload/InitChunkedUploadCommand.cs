using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.InitChunkedUpload;

public sealed record InitChunkedUploadCommand(
    InstallationIdentifier InstallationId,
    FileName FileName,
    ContentType ContentType,
    FileSize TotalSize,
    int TotalChunks,
    PhotoType PhotoType,
    Caption? Caption,
    Description? Description,
    Latitude? Latitude,
    Longitude? Longitude,
    double? Altitude,
    HorizontalAccuracy? HorizontalAccuracy,
    GpsSource? GpsSource) : ICommand<Guid>;
