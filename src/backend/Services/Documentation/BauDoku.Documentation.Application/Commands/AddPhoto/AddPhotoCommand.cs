using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Documentation.Application.Commands.AddPhoto;

public sealed record AddPhotoCommand(
    Guid InstallationId,
    string FileName,
    string ContentType,
    long FileSize,
    string PhotoType,
    string? Caption,
    string? Description,
    double? Latitude,
    double? Longitude,
    double? Altitude,
    double? HorizontalAccuracy,
    string? GpsSource,
    Stream Stream,
    DateTime? TakenAt = null) : ICommand<Guid>;
