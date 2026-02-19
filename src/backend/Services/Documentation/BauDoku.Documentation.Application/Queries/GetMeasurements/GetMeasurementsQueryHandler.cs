using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Mapping;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries.GetMeasurements;

public sealed class GetMeasurementsQueryHandler(IInstallationRepository installations)
    : IQueryHandler<GetMeasurementsQuery, IReadOnlyList<MeasurementDto>>
{
    public async Task<IReadOnlyList<MeasurementDto>> Handle(GetMeasurementsQuery query, CancellationToken cancellationToken = default)
    {
        var installationId = InstallationIdentifier.From(query.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken);

        return installation.Measurements.Select(m => m.ToDto(installation.Id.Value)).ToList();
    }
}
