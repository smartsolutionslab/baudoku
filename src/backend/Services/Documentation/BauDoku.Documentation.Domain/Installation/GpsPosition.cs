using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record GpsPosition : IValueObject
{
    public Latitude Latitude { get; }
    public Longitude Longitude { get; }
    public Altitude? Altitude { get; }
    public HorizontalAccuracy HorizontalAccuracy { get; }
    public GpsSource Source { get; }
    public CorrectionService? CorrectionService { get; }
    public RtkFixStatus? RtkFixStatus { get; }
    public SatelliteCount? SatelliteCount { get; }
    public Hdop? Hdop { get; }
    public CorrectionAge? CorrectionAge { get; }

    private GpsPosition(
        Latitude latitude,
        Longitude longitude,
        Altitude? altitude,
        HorizontalAccuracy horizontalAccuracy,
        GpsSource source,
        CorrectionService? correctionService,
        RtkFixStatus? rtkFixStatus,
        SatelliteCount? satelliteCount,
        Hdop? hdop,
        CorrectionAge? correctionAge)
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
        Latitude latitude,
        Longitude longitude,
        Altitude? altitude,
        HorizontalAccuracy horizontalAccuracy,
        GpsSource source,
        CorrectionService? correctionService = null,
        RtkFixStatus? rtkFixStatus = null,
        SatelliteCount? satelliteCount = null,
        Hdop? hdop = null,
        CorrectionAge? correctionAge = null)
    {
        return new GpsPosition(
            latitude, longitude, altitude, horizontalAccuracy, source,
            correctionService, rtkFixStatus, satelliteCount, hdop, correctionAge);
    }

    private const double HighAccuracyThresholdMeters = 1.0;
    private const double MediumAccuracyThresholdMeters = 5.0;
    private const double LowAccuracyThresholdMeters = 30.0;
    private const double GoodHdopThreshold = 2.0;
    private const double PoorHdopThreshold = 5.0;
    private const int GoodSatelliteCount = 8;
    private const int PoorSatelliteCount = 4;

    public GpsQualityGrade CalculateQualityGrade()
    {
        var baseGrade = HorizontalAccuracy.Value switch
        {
            < HighAccuracyThresholdMeters => 0,
            < MediumAccuracyThresholdMeters => 1,
            < LowAccuracyThresholdMeters => 2,
            _ => 3
        };

        var bonus = 0;
        if (Hdop is not null && Hdop.Value < GoodHdopThreshold) bonus++;
        if (SatelliteCount is not null && SatelliteCount.Value >= GoodSatelliteCount) bonus++;
        if (CorrectionService is not null) bonus++;

        var penalty = 0;
        if (Hdop is not null && Hdop.Value > PoorHdopThreshold) penalty++;
        if (SatelliteCount is not null && SatelliteCount.Value < PoorSatelliteCount) penalty++;

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
