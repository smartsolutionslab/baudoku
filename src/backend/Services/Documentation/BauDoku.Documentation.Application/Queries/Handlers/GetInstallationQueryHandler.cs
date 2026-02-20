using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Mapping;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries.Handlers;

public sealed class GetInstallationQueryHandler(IInstallationRepository installations)
    : IQueryHandler<GetInstallationQuery, InstallationDto>
{
    public async Task<InstallationDto> Handle(GetInstallationQuery query, CancellationToken cancellationToken = default)
    {
        var installation = await installations.GetByIdReadOnlyAsync(query.InstallationId, cancellationToken);

        return installation.ToDto();
    }
}
