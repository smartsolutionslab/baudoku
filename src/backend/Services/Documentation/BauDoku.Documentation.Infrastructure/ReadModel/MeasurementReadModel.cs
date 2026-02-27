namespace BauDoku.Documentation.Infrastructure.ReadModel;

public sealed class MeasurementReadModel
{
    public Guid Id { get; set; }
    public Guid InstallationId { get; set; }
    public string Type { get; set; } = default!;
    public double Value { get; set; }
    public string Unit { get; set; } = default!;
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public string Result { get; set; } = default!;
    public string? Notes { get; set; }
    public DateTime MeasuredAt { get; set; }

    public InstallationReadModel Installation { get; set; } = default!;
}
