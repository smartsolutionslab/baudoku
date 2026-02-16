using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.GetInstallation;

public sealed class GetInstallationQueryHandler(IInstallationRepository installations)
    : IQueryHandler<GetInstallationQuery, InstallationDto?>
{
    public async Task<InstallationDto?> Handle(GetInstallationQuery query, CancellationToken cancellationToken)
    {
        var installationId = InstallationIdentifier.From(query.InstallationId);
        var installation = await installations.GetByIdReadOnlyAsync(installationId, cancellationToken);

        if (installation is null)
            return null;

        var photos = installation.Photos.Select(p => new PhotoDto(
            p.Id.Value,
            installation.Id.Value,
            p.FileName,
            p.BlobUrl,
            p.ContentType,
            p.FileSize,
            p.PhotoType.Value,
            p.Caption?.Value,
            p.Description?.Value,
            p.Position?.Latitude,
            p.Position?.Longitude,
            p.Position?.Altitude,
            p.Position?.HorizontalAccuracy,
            p.Position?.Source,
            p.Position?.CorrectionService,
            p.Position?.RtkFixStatus,
            p.Position?.SatelliteCount,
            p.Position?.Hdop,
            p.Position?.CorrectionAge,
            p.TakenAt)).ToList();

        var measurements = installation.Measurements.Select(m => new MeasurementDto(
            m.Id.Value,
            installation.Id.Value,
            m.Type.Value,
            m.Value.Value,
            m.Value.Unit,
            m.Value.MinThreshold,
            m.Value.MaxThreshold,
            m.Result.Value,
            m.Notes,
            m.MeasuredAt)).ToList();

        return new InstallationDto(
            installation.Id.Value,
            installation.ProjectId.Value,
            installation.ZoneId?.Value,
            installation.Type.Value,
            installation.Status.Value,
            installation.Position.Latitude,
            installation.Position.Longitude,
            installation.Position.Altitude,
            installation.Position.HorizontalAccuracy,
            installation.Position.Source,
            installation.Position.CorrectionService,
            installation.Position.RtkFixStatus,
            installation.Position.SatelliteCount,
            installation.Position.Hdop,
            installation.Position.CorrectionAge,
            installation.QualityGrade.Value,
            installation.Description?.Value,
            installation.CableSpec?.CableType,
            installation.CableSpec?.CrossSection,
            installation.CableSpec?.Color,
            installation.CableSpec?.ConductorCount,
            installation.Depth?.ValueInMillimeters,
            installation.Manufacturer?.Value,
            installation.ModelName?.Value,
            installation.SerialNumber?.Value,
            installation.CreatedAt,
            installation.CompletedAt,
            photos,
            measurements);
    }
}
