using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.AddPhoto;

public sealed record AddPhotoCommand(
    InstallationIdentifier InstallationId,
    FileName FileName,
    ContentType ContentType,
    FileSize FileSize,
    PhotoType PhotoType,
    Caption? Caption,
    Description? Description,
    Latitude? Latitude,
    Longitude? Longitude,
    double? Altitude,
    HorizontalAccuracy? HorizontalAccuracy,
    GpsSource? GpsSource,
    Stream Stream,
    DateTime? TakenAt = null) : ICommand<Guid>;
