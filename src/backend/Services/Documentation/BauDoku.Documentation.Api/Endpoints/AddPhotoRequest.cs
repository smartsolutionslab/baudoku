namespace BauDoku.Documentation.Api.Endpoints;

public sealed record AddPhotoRequest(
    string? PhotoType,
    string? Caption,
    string? Description,
    GpsPositionRequest? Position,
    DateTime? TakenAt);
