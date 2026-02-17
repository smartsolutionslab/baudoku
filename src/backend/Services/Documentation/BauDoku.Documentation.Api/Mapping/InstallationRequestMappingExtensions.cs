using BauDoku.Documentation.Application.Commands.RecordMeasurement;
using BauDoku.Documentation.Application.Commands.UpdateInstallation;
using BauDoku.Documentation.Api.Endpoints;

namespace BauDoku.Documentation.Api.Mapping;

public static class InstallationRequestMappingExtensions
{
    public static UpdateInstallationCommand ToCommand(this UpdateInstallationRequest request, Guid installationId) =>
        new(installationId,
            request.Latitude,
            request.Longitude,
            request.Altitude,
            request.HorizontalAccuracy,
            request.GpsSource,
            request.CorrectionService,
            request.RtkFixStatus,
            request.SatelliteCount,
            request.Hdop,
            request.CorrectionAge,
            request.Description,
            request.CableType,
            request.CrossSection,
            request.CableColor,
            request.ConductorCount,
            request.DepthMm,
            request.Manufacturer,
            request.ModelName,
            request.SerialNumber);

    public static RecordMeasurementCommand ToCommand(this RecordMeasurementRequest request, Guid installationId) =>
        new(installationId,
            request.Type,
            request.Value,
            request.Unit,
            request.MinThreshold,
            request.MaxThreshold,
            request.Notes);
}
