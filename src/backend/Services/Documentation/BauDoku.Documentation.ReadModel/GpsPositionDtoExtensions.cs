using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.ReadModel;

public static class GpsPositionDtoExtensions
{
    public static GpsPositionDto ToGpsDto(this GpsPosition position) =>
        new(position.Latitude.Value,
            position.Longitude.Value,
            position.Altitude?.Value,
            position.HorizontalAccuracy.Value,
            position.Source.Value,
            position.CorrectionService?.Value,
            position.RtkFixStatus?.Value,
            position.SatelliteCount?.Value,
            position.Hdop?.Value,
            position.CorrectionAge?.Value);

    public static GpsPosition ToDomain(this GpsPositionDto dto) =>
        GpsPosition.Create(
            Latitude.From(dto.Latitude),
            Longitude.From(dto.Longitude),
            Altitude.FromNullable(dto.Altitude),
            HorizontalAccuracy.From(dto.HorizontalAccuracy),
            GpsSource.From(dto.GpsSource),
            CorrectionService.FromNullable(dto.CorrectionService),
            RtkFixStatus.FromNullable(dto.RtkFixStatus),
            SatelliteCount.FromNullable(dto.SatelliteCount),
            Hdop.FromNullable(dto.Hdop),
            CorrectionAge.FromNullable(dto.CorrectionAge));
}
