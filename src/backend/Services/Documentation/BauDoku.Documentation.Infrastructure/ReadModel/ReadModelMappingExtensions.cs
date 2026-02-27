using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Infrastructure.ReadModel;

internal static class ReadModelMappingExtensions
{
    internal static IQueryable<InstallationListItemDto> SelectListItems(this IQueryable<InstallationReadModel> query) =>
        query.Select(i => new InstallationListItemDto(
            i.Id,
            i.ProjectId,
            i.Type,
            i.Status,
            i.QualityGrade,
            i.Latitude,
            i.Longitude,
            i.Description,
            i.CreatedAt,
            i.PhotoCount,
            i.MeasurementCount));

    internal static IQueryable<PhotoDto> SelectPhotoDtos(this IQueryable<PhotoReadModel> query) =>
        query.Select(p => new PhotoDto(
            p.Id,
            p.InstallationId,
            p.FileName,
            p.BlobUrl,
            p.ContentType,
            p.FileSize,
            p.PhotoType,
            p.Caption,
            p.Description,
            p.Latitude,
            p.Longitude,
            p.Altitude,
            p.HorizontalAccuracy,
            p.GpsSource,
            p.CorrectionService,
            p.RtkFixStatus,
            p.SatelliteCount,
            p.Hdop,
            p.CorrectionAge,
            p.TakenAt));
}
