namespace BauDoku.Documentation.Infrastructure.ReadModel;

public sealed class PhotoReadModel
{
    public Guid Id { get; set; }
    public Guid InstallationId { get; set; }
    public string FileName { get; set; } = default!;
    public string BlobUrl { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }
    public string PhotoType { get; set; } = default!;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Altitude { get; set; }
    public double? HorizontalAccuracy { get; set; }
    public string? GpsSource { get; set; }
    public string? CorrectionService { get; set; }
    public string? RtkFixStatus { get; set; }
    public int? SatelliteCount { get; set; }
    public double? Hdop { get; set; }
    public double? CorrectionAge { get; set; }
    public DateTime TakenAt { get; set; }

    public InstallationReadModel Installation { get; set; } = default!;
}
