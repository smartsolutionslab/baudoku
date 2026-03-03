namespace BauDoku.Documentation.Application.ReadModel;

public sealed record PhotoDto(
    Guid Id,
    Guid InstallationId,
    string FileName,
    string BlobUrl,
    string ContentType,
    long FileSize,
    string PhotoType,
    string? Caption,
    string? Description,
    GpsPositionDto? Position,
    DateTime TakenAt);
