namespace BauDoku.Documentation.Api.Endpoints;

public sealed record AddPhotoRequest(
    string? PhotoType,
    string? Caption,
    string? Description,
    double? Latitude,
    double? Longitude,
    double? Altitude,
    double? HorizontalAccuracy,
    string? GpsSource,
    DateTime? TakenAt);
