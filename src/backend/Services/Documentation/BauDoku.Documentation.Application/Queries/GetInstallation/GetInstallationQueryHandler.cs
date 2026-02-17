using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Mapping;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Queries.GetInstallation;

public sealed class GetInstallationQueryHandler(IInstallationRepository installations)
    : IQueryHandler<GetInstallationQuery, InstallationDto?>
{
    public async Task<InstallationDto?> Handle(GetInstallationQuery query, CancellationToken cancellationToken = default)
    {
        var installationId = InstallationIdentifier.From(query.InstallationId);
        var installation = await installations.GetByIdReadOnlyAsync(installationId, cancellationToken);

        return installation?.ToDto();
    }
}
