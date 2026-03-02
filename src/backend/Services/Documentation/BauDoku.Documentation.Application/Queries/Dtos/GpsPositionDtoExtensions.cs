using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries.Dtos;

public static class GpsPositionDtoExtensions
{
    public static GpsPositionDto ToGpsDto(this GpsPosition position) =>
        new(position.Latitude.Value,
            position.Longitude.Value,
            position.Altitude,
            position.HorizontalAccuracy.Value,
            position.Source.Value,
            position.CorrectionService?.Value,
            position.RtkFixStatus?.Value,
            position.SatelliteCount,
            position.Hdop,
            position.CorrectionAge);

    public static GpsPosition ToDomain(this GpsPositionDto dto) =>
        GpsPosition.Create(
            Latitude.From(dto.Latitude),
            Longitude.From(dto.Longitude),
            dto.Altitude,
            HorizontalAccuracy.From(dto.HorizontalAccuracy),
            GpsSource.From(dto.GpsSource),
            CorrectionService.FromNullable(dto.CorrectionService),
            RtkFixStatus.FromNullable(dto.RtkFixStatus),
            dto.SatelliteCount,
            dto.Hdop,
            dto.CorrectionAge);
}
