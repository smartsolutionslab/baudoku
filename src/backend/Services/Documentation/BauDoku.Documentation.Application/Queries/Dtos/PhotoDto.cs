namespace BauDoku.Documentation.Application.Queries.Dtos;

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
    double? Latitude,
    double? Longitude,
    DateTime TakenAt);
