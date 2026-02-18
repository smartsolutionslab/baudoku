using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record GpsPosition : IValueObject
{
    public Latitude Latitude { get; }
    public Longitude Longitude { get; }
    public double? Altitude { get; }
    public HorizontalAccuracy HorizontalAccuracy { get; }
    public GpsSource Source { get; }
    public CorrectionService? CorrectionService { get; }
    public RtkFixStatus? RtkFixStatus { get; }
    public int? SatelliteCount { get; }
    public double? Hdop { get; }
    public double? CorrectionAge { get; }

    private GpsPosition(
        Latitude latitude,
        Longitude longitude,
        double? altitude,
        HorizontalAccuracy horizontalAccuracy,
        GpsSource source,
        CorrectionService? correctionService,
        RtkFixStatus? rtkFixStatus,
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
        return new GpsPosition(
            Latitude.From(latitude),
            Longitude.From(longitude),
            altitude,
            HorizontalAccuracy.From(horizontalAccuracy),
            GpsSource.From(source),
            correctionService is not null ? ValueObjects.CorrectionService.From(correctionService) : null,
            rtkFixStatus is not null ? ValueObjects.RtkFixStatus.From(rtkFixStatus) : null,
            satelliteCount, hdop, correctionAge);
    }

    public GpsQualityGrade CalculateQualityGrade()
    {
        var baseGrade = HorizontalAccuracy.Value switch
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
