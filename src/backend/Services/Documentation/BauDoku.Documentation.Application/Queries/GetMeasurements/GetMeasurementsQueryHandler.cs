using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.GetMeasurements;

public sealed class GetMeasurementsQueryHandler
    : IQueryHandler<GetMeasurementsQuery, IReadOnlyList<MeasurementDto>>
{
    private readonly IInstallationRepository installations;

    public GetMeasurementsQueryHandler(IInstallationRepository installations)
    {
        this.installations = installations;
    }

    public async Task<IReadOnlyList<MeasurementDto>> Handle(
        GetMeasurementsQuery query, CancellationToken cancellationToken)
    {
        var installationId = InstallationIdentifier.From(query.InstallationId);
        var installation = await installations.GetByIdAsync(installationId, cancellationToken)
            ?? throw new InvalidOperationException($"Installation mit ID {query.InstallationId} nicht gefunden.");

        return installation.Measurements.Select(m => new MeasurementDto(
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
    }
}
