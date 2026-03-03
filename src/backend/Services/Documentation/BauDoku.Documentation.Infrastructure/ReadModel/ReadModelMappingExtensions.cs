using SmartSolutionsLab.BauDoku.Documentation.ReadModel;

namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure.ReadModel;

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

    internal static InstallationDto ToInstallationDto(this InstallationReadModel i, List<PhotoDto> photos, List<MeasurementDto> measurements) =>
        new(i.Id,
            i.ProjectId,
            i.ZoneId,
            i.Type,
            i.Status,
            new GpsPositionDto(
                i.Latitude,
                i.Longitude,
                i.Altitude,
                i.HorizontalAccuracy,
                i.GpsSource,
                i.CorrectionService,
                i.RtkFixStatus,
                i.SatelliteCount,
                i.Hdop,
                i.CorrectionAge),
            i.QualityGrade,
            i.Description,
            i.CableType,
            i.CrossSection,
            i.CableColor,
            i.ConductorCount,
            i.DepthMm,
            i.Manufacturer,
            i.ModelName,
            i.SerialNumber,
            i.CreatedAt,
            i.CompletedAt,
            photos,
            measurements);

    internal static IQueryable<MeasurementDto> SelectMeasurementDtos(this IQueryable<MeasurementReadModel> query) =>
        query.Select(m => new MeasurementDto(
            m.Id,
            m.InstallationId,
            m.Type,
            m.Value,
            m.Unit,
            m.MinThreshold,
            m.MaxThreshold,
            m.Result,
            m.Notes,
            m.MeasuredAt));

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
            p.Latitude.HasValue
                ? new GpsPositionDto(
                    p.Latitude.Value,
                    p.Longitude!.Value,
                    p.Altitude,
                    p.HorizontalAccuracy!.Value,
                    p.GpsSource!,
                    p.CorrectionService,
                    p.RtkFixStatus,
                    p.SatelliteCount,
                    p.Hdop,
                    p.CorrectionAge)
                : null,
            p.TakenAt));
}
