using System.Diagnostics.Metrics;

namespace BauDoku.Documentation.Application.Diagnostics;

public static class DocumentationMetrics
{
    private static readonly Meter Meter = new("BauDoku.Documentation");

    public static readonly Counter<long> InstallationsDocumented = Meter.CreateCounter<long>("baudoku.documentation.installations_documented", description: "Number of installations documented");

    public static readonly Counter<long> PhotosAdded = Meter.CreateCounter<long>("baudoku.documentation.photos_added", description: "Number of photos added");

    public static readonly Counter<long> PhotosRemoved = Meter.CreateCounter<long>("baudoku.documentation.photos_removed", description: "Number of photos removed");

    public static readonly Counter<long> MeasurementsRecorded = Meter.CreateCounter<long>("baudoku.documentation.measurements_recorded", description: "Number of measurements recorded");

    public static readonly Counter<long> MeasurementsRemoved = Meter.CreateCounter<long>("baudoku.documentation.measurements_removed", description: "Number of measurements removed");

    public static readonly Histogram<double> GpsHorizontalAccuracy = Meter.CreateHistogram<double>("baudoku.documentation.gps_horizontal_accuracy_meters", unit: "m", description: "GPS horizontal accuracy in meters");

    public static readonly Histogram<long> PhotoFileSize = Meter.CreateHistogram<long>("baudoku.documentation.photo_file_size_bytes", unit: "By", description: "Photo file size in bytes");
}
