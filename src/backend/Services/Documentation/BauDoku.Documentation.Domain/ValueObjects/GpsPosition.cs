using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record GpsPosition : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }
    public double? Altitude { get; }
    public double HorizontalAccuracy { get; }
    public string Source { get; }
    public string? CorrectionService { get; }
    public string? RtkFixStatus { get; }
    public int? SatelliteCount { get; }
    public double? Hdop { get; }
    public double? CorrectionAge { get; }

    public GpsPosition(
        double latitude,
        double longitude,
        double? altitude,
        double horizontalAccuracy,
        string source,
        string? correctionService = null,
        string? rtkFixStatus = null,
        int? satelliteCount = null,
        double? hdop = null,
        double? correctionAge = null)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Breitengrad muss zwischen -90 und 90 liegen.");
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Laengengrad muss zwischen -180 und 180 liegen.");
        if (horizontalAccuracy <= 0)
            throw new ArgumentOutOfRangeException(nameof(horizontalAccuracy), "Horizontale Genauigkeit muss groesser als 0 sein.");
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("GPS-Quelle darf nicht leer sein.", nameof(source));

        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        HorizontalAccuracy = horizontalAccuracy;
        Source = source;
        CorrectionService = correctionService;
        RtkFixStatus = rtkFixStatus;
        SatelliteCount = satelliteCount;
        Hdop = hdop;
        CorrectionAge = correctionAge;
    }

    public GpsQualityGrade CalculateQualityGrade()
    {
        var baseGrade = HorizontalAccuracy switch
        {
            < 1.0 => 0,
            < 5.0 => 1,
            < 30.0 => 2,
            _ => 3
        };

        var bonus = 0;
        if (Hdop.HasValue && Hdop.Value < 2.0) bonus++;
        if (SatelliteCount.HasValue && SatelliteCount.Value >= 8) bonus++;
        if (CorrectionService is not null) bonus++;

        var penalty = 0;
        if (Hdop.HasValue && Hdop.Value > 5.0) penalty++;
        if (SatelliteCount.HasValue && SatelliteCount.Value < 4) penalty++;

        var adjustment = Math.Clamp(bonus - penalty, -1, 1);
        var finalGrade = Math.Clamp(baseGrade - adjustment, 0, 3);

        return finalGrade switch
        {
            0 => GpsQualityGrade.A,
            1 => GpsQualityGrade.B,
            2 => GpsQualityGrade.C,
            _ => GpsQualityGrade.D
        };
    }
}
