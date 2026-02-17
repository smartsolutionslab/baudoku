namespace BauDoku.Documentation.Api.Endpoints;

public sealed record RecordMeasurementRequest(
    string Type,
    double Value,
    string Unit,
    double? MinThreshold,
    double? MaxThreshold,
    string? Notes);
