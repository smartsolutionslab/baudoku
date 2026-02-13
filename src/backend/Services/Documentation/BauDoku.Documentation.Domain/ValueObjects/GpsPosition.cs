using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

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

    private GpsPosition(
        double latitude,
        double longitude,
        double? altitude,
        double horizontalAccuracy,
        string source,
        string? correctionService,
        string? rtkFixStatus,
        int? satelliteCount,
        double? hdop,
        double? correctionAge)
    {
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

    public static GpsPosition Create(
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
        Ensure.That(latitude).IsBetween(-90.0, 90.0, "Breitengrad muss zwischen -90 und 90 liegen.");
        Ensure.That(longitude).IsBetween(-180.0, 180.0, "Laengengrad muss zwischen -180 und 180 liegen.");
        Ensure.That(horizontalAccuracy).IsPositive("Horizontale Genauigkeit muss groesser als 0 sein.");
        Ensure.That(source).IsNotNullOrWhiteSpace("GPS-Quelle darf nicht leer sein.");
        return new GpsPosition(latitude, longitude, altitude, horizontalAccuracy, source,
            correctionService, rtkFixStatus, satelliteCount, hdop, correctionAge);
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
