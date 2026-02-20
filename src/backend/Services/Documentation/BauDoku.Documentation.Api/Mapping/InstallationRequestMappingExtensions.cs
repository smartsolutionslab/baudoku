using BauDoku.Documentation.Application.Commands.AddPhoto;
using BauDoku.Documentation.Application.Commands.RecordMeasurement;
using BauDoku.Documentation.Application.Commands.UpdateInstallation;
using BauDoku.Documentation.Api.Endpoints;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Api.Mapping;

public static class InstallationRequestMappingExtensions
{
    public static AddPhotoCommand ToCommand(this AddPhotoRequest request, Guid installationId, IFormFile file, Stream stream) =>
        new(InstallationIdentifier.From(installationId),
            FileName.From(file.FileName),
            ContentType.From(file.ContentType),
            FileSize.From(file.Length),
            PhotoType.From(request.PhotoType ?? "other"),
            Caption.FromNullable(request.Caption),
            Description.FromNullable(request.Description),
            Latitude.FromNullable(request.Latitude),
            Longitude.FromNullable(request.Longitude),
            request.Altitude,
            HorizontalAccuracy.FromNullable(request.HorizontalAccuracy),
            GpsSource.FromNullable(request.GpsSource),
            stream,
            request.TakenAt);

    public static UpdateInstallationCommand ToCommand(this UpdateInstallationRequest request, Guid installationId) =>
        new(InstallationIdentifier.From(installationId),
            Latitude.FromNullable(request.Latitude),
            Longitude.FromNullable(request.Longitude),
            request.Altitude,
            HorizontalAccuracy.FromNullable(request.HorizontalAccuracy),
            GpsSource.FromNullable(request.GpsSource),
            CorrectionService.FromNullable(request.CorrectionService),
            RtkFixStatus.FromNullable(request.RtkFixStatus),
            request.SatelliteCount,
            request.Hdop,
            request.CorrectionAge,
            Description.FromNullable(request.Description),
            CableType.FromNullable(request.CableType),
            CrossSection.FromNullable(request.CrossSection),
            CableColor.FromNullable(request.CableColor),
            request.ConductorCount,
            request.DepthMm,
            Manufacturer.FromNullable(request.Manufacturer),
            ModelName.FromNullable(request.ModelName),
            SerialNumber.FromNullable(request.SerialNumber));

    public static RecordMeasurementCommand ToCommand(this RecordMeasurementRequest request, Guid installationId) =>
        new(InstallationIdentifier.From(installationId),
            MeasurementType.From(request.Type),
            request.Value,
            MeasurementUnit.From(request.Unit),
            request.MinThreshold,
            request.MaxThreshold,
            request.Notes);
}
