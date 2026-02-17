using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.Entities;

namespace BauDoku.Documentation.Application.Mapping;

public static class InstallationMappingExtensions
{
    public static InstallationDto ToDto(this Installation installation) =>
        new(installation.Id.Value,
            installation.ProjectId.Value,
            installation.ZoneId?.Value,
            installation.Type.Value,
            installation.Status.Value,
            installation.Position.Latitude,
            installation.Position.Longitude,
            installation.Position.Altitude,
            installation.Position.HorizontalAccuracy,
            installation.Position.Source.Value,
            installation.Position.CorrectionService?.Value,
            installation.Position.RtkFixStatus?.Value,
            installation.Position.SatelliteCount,
            installation.Position.Hdop,
            installation.Position.CorrectionAge,
            installation.QualityGrade.Value,
            installation.Description?.Value,
            installation.CableSpec?.CableType.Value,
            installation.CableSpec?.CrossSection?.Value,
            installation.CableSpec?.Color?.Value,
            installation.CableSpec?.ConductorCount,
            installation.Depth?.ValueInMillimeters,
            installation.Manufacturer?.Value,
            installation.ModelName?.Value,
            installation.SerialNumber?.Value,
            installation.CreatedAt,
            installation.CompletedAt,
            installation.Photos.Select(p => p.ToDto(installation.Id.Value)).ToList(),
            installation.Measurements.Select(m => m.ToDto(installation.Id.Value)).ToList());

    public static PhotoDto ToDto(this Photo photo, Guid installationId) =>
        new(photo.Id.Value,
            installationId,
            photo.FileName.Value,
            photo.BlobUrl.Value,
            photo.ContentType.Value,
            photo.FileSize.Value,
            photo.PhotoType.Value,
            photo.Caption?.Value,
            photo.Description?.Value,
            photo.Position?.Latitude,
            photo.Position?.Longitude,
            photo.Position?.Altitude,
            photo.Position?.HorizontalAccuracy,
            photo.Position?.Source.Value,
            photo.Position?.CorrectionService?.Value,
            photo.Position?.RtkFixStatus?.Value,
            photo.Position?.SatelliteCount,
            photo.Position?.Hdop,
            photo.Position?.CorrectionAge,
            photo.TakenAt);

    public static MeasurementDto ToDto(this Measurement measurement, Guid installationId) =>
        new(measurement.Id.Value,
            installationId,
            measurement.Type.Value,
            measurement.Value.Value,
            measurement.Value.Unit.Value,
            measurement.Value.MinThreshold,
            measurement.Value.MaxThreshold,
            measurement.Result.Value,
            measurement.Notes?.Value,
            measurement.MeasuredAt);
}
