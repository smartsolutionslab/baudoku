using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands;

public sealed record RemoveMeasurementCommand(
    InstallationIdentifier InstallationId,
    MeasurementIdentifier MeasurementId) : ICommand;
