using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.InitChunkedUpload;

public sealed record InitChunkedUploadCommand(
    Guid InstallationId,
    string FileName,
    string ContentType,
    long TotalSize,
    int TotalChunks,
    string PhotoType,
    string? Caption,
    string? Description,
    double? Latitude,
    double? Longitude,
    double? Altitude,
    double? HorizontalAccuracy,
    string? GpsSource) : ICommand<Guid>;
