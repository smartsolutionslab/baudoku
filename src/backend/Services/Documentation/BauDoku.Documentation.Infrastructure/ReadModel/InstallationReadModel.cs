namespace BauDoku.Documentation.Infrastructure.ReadModel;

public sealed class InstallationReadModel
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ZoneId { get; set; }
    public string Type { get; set; } = default!;
    public string Status { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Altitude { get; set; }
    public double HorizontalAccuracy { get; set; }
    public string GpsSource { get; set; } = default!;
    public string? CorrectionService { get; set; }
    public string? RtkFixStatus { get; set; }
    public int? SatelliteCount { get; set; }
    public double? Hdop { get; set; }
    public double? CorrectionAge { get; set; }
    public string QualityGrade { get; set; } = default!;
    public string? Description { get; set; }
    public string? CableType { get; set; }
    public decimal? CrossSection { get; set; }
    public string? CableColor { get; set; }
    public int? ConductorCount { get; set; }
    public int? DepthMm { get; set; }
    public string? Manufacturer { get; set; }
    public string? ModelName { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsDeleted { get; set; }
    public int PhotoCount { get; set; }
    public int MeasurementCount { get; set; }

    public List<PhotoReadModel> Photos { get; set; } = [];
    public List<MeasurementReadModel> Measurements { get; set; } = [];
}
