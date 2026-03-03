using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands;

public sealed record RemoveMeasurementCommand(
    InstallationIdentifier InstallationId,
    MeasurementIdentifier MeasurementId) : ICommand;
