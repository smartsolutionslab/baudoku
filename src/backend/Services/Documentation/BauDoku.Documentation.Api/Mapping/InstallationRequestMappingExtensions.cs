using SmartSolutionsLab.BauDoku.Documentation.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Api.Endpoints;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Api.Mapping;

public static class InstallationRequestMappingExtensions
{
    public static GpsPosition ToDomain(this GpsPositionRequest request) => GpsPosition.Create(
            Latitude.From(request.Latitude),
            Longitude.From(request.Longitude),
            Altitude.FromNullable(request.Altitude),
            HorizontalAccuracy.From(request.HorizontalAccuracy),
            GpsSource.From(request.GpsSource),
            CorrectionService.FromNullable(request.CorrectionService),
            RtkFixStatus.FromNullable(request.RtkFixStatus),
            SatelliteCount.FromNullable(request.SatelliteCount),
            Hdop.FromNullable(request.Hdop),
            CorrectionAge.FromNullable(request.CorrectionAge));

    public static DocumentInstallationCommand ToCommand(this CreateInstallationRequest request) => new(
        ProjectIdentifier.From(request.ProjectId),
            ZoneIdentifier.FromNullable(request.ZoneId),
            InstallationType.From(request.Type),
            request.Position?.ToDomain(),
            Description.FromNullable(request.Description),
            CableType.FromNullable(request.CableType),
            CrossSection.FromNullable(request.CrossSection),
            CableColor.FromNullable(request.CableColor),
            ConductorCount.FromNullable(request.ConductorCount),
            Depth.FromNullable(request.DepthMm),
            Manufacturer.FromNullable(request.Manufacturer),
            ModelName.FromNullable(request.ModelName),
            SerialNumber.FromNullable(request.SerialNumber));

    public static AddPhotoCommand ToCommand(this AddPhotoRequest request, Guid installationId, IFormFile file, Stream stream) => new(
        InstallationIdentifier.From(installationId),
            FileName.From(file.FileName),
            ContentType.From(file.ContentType),
            FileSize.From(file.Length),
            PhotoType.From(request.PhotoType ?? "other"),
            Caption.FromNullable(request.Caption),
            Description.FromNullable(request.Description),
            request.Position?.ToDomain(),
            stream,
            request.TakenAt);

    public static UpdateInstallationCommand ToCommand(this UpdateInstallationRequest request, Guid installationId) =>
        new(InstallationIdentifier.From(installationId),
            request.Position?.ToDomain(),
            Description.FromNullable(request.Description),
            CableType.FromNullable(request.CableType),
            CrossSection.FromNullable(request.CrossSection),
            CableColor.FromNullable(request.CableColor),
            ConductorCount.FromNullable(request.ConductorCount),
            Depth.FromNullable(request.DepthMm),
            Manufacturer.FromNullable(request.Manufacturer),
            ModelName.FromNullable(request.ModelName),
            SerialNumber.FromNullable(request.SerialNumber));

    public static InitChunkedUploadCommand ToCommand(this InitChunkedUploadRequest request) => new(
        InstallationIdentifier.From(request.InstallationId),
            FileName.From(request.FileName),
            ContentType.From(request.ContentType),
            FileSize.From(request.TotalSize),
            ChunkCount.From(request.TotalChunks),
            PhotoType.From(request.PhotoType),
            Caption.FromNullable(request.Caption),
            Description.FromNullable(request.Description),
            request.Position?.ToDomain());

    public static RecordMeasurementCommand ToCommand(this RecordMeasurementRequest request, Guid installationId) => new(
        InstallationIdentifier.From(installationId),
            MeasurementType.From(request.Type),
            request.Value,
            MeasurementUnit.From(request.Unit),
            request.MinThreshold,
            request.MaxThreshold,
            Notes.FromNullable(request.Notes));
}
