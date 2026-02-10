using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.GetInstallation;

public sealed class GetInstallationQueryHandler
    : IQueryHandler<GetInstallationQuery, InstallationDto?>
{
    private readonly IInstallationRepository _installationRepository;

    public GetInstallationQueryHandler(IInstallationRepository installationRepository)
    {
        _installationRepository = installationRepository;
    }

    public async Task<InstallationDto?> Handle(GetInstallationQuery query, CancellationToken cancellationToken)
    {
        var installationId = new InstallationId(query.InstallationId);
        var installation = await _installationRepository.GetByIdAsync(installationId, cancellationToken);

        if (installation is null)
            return null;

        return new InstallationDto(
            installation.Id.Value,
            installation.ProjectId,
            installation.ZoneId,
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
            installation.Photos.Count,
            installation.Measurements.Count);
    }
}
