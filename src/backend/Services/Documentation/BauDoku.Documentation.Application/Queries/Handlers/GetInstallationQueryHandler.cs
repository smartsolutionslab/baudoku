using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.ReadModel;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetInstallationQueryHandler(IInstallationReadRepository installations): IQueryHandler<GetInstallationQuery, InstallationDto>
{
    public async Task<InstallationDto> Handle(GetInstallationQuery query, CancellationToken cancellationToken = default)
    {
        return await installations.GetByIdAsync(query.InstallationId, cancellationToken);
    }
}
