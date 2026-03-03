namespace BauDoku.Documentation.ReadModel;

public sealed record MeasurementDto(
    Guid Id,
    Guid InstallationId,
    string Type,
    double Value,
    string Unit,
    double? MinThreshold,
    double? MaxThreshold,
    string Result,
    string? Notes,
    DateTime MeasuredAt);
