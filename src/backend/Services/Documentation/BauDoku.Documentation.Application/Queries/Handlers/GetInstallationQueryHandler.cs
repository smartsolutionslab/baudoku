using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetInstallationQueryHandler(IInstallationReadRepository installations): IQueryHandler<GetInstallationQuery, InstallationDto>
{
    public async Task<InstallationDto> Handle(GetInstallationQuery query, CancellationToken cancellationToken = default)
    {
        return await installations.With(query.InstallationId, cancellationToken);
    }
}
