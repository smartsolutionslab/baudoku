using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands;

public sealed record RecordMeasurementCommand(
    InstallationIdentifier InstallationId,
    MeasurementType Type,
    double Value,
    MeasurementUnit Unit,
    double? MinThreshold,
    double? MaxThreshold,
    Notes? Notes) : ICommand<MeasurementIdentifier>;
