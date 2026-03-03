using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetMeasurementsQueryHandler(IInstallationReadRepository installations): IQueryHandler<GetMeasurementsQuery, IReadOnlyList<MeasurementDto>>
{
    public async Task<IReadOnlyList<MeasurementDto>> Handle(GetMeasurementsQuery query, CancellationToken cancellationToken = default)
    {
        return await installations.GetMeasurementsAsync(query.InstallationId, cancellationToken);
    }
}
