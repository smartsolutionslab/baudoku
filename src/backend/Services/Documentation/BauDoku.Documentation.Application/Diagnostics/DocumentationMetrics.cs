using System.Diagnostics.Metrics;

namespace BauDoku.Documentation.Application.Diagnostics;

public static class DocumentationMetrics
{
    private static readonly Meter Meter = new("BauDoku.Documentation");

    public static readonly Counter<long> InstallationsDocumented =
        Meter.CreateCounter<long>("baudoku.documentation.installations_documented", description: "Number of installations documented");

    public static readonly Counter<long> PhotosAdded =
        Meter.CreateCounter<long>("baudoku.documentation.photos_added", description: "Number of photos added");

    public static readonly Counter<long> PhotosRemoved =
        Meter.CreateCounter<long>("baudoku.documentation.photos_removed", description: "Number of photos removed");

    public static readonly Counter<long> MeasurementsRecorded =
        Meter.CreateCounter<long>("baudoku.documentation.measurements_recorded", description: "Number of measurements recorded");

    public static readonly Counter<long> MeasurementsRemoved =
        Meter.CreateCounter<long>("baudoku.documentation.measurements_removed", description: "Number of measurements removed");
}
