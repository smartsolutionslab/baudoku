using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class UpdateInstallationCommandHandler(IInstallationRepository installations)
    : ICommandHandler<UpdateInstallationCommand>
{
    public async Task Handle(UpdateInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var (installationId, position, description, cableType, crossSection, cableColor, conductorCount, depth, manufacturer, modelName, serialNumber) = command;
        var installation = await installations.With(installationId, cancellationToken);

        if (position is not null)
        {
            installation.UpdatePosition(position);
        }

        if (description is not null)
        {
            installation.UpdateDescription(description);
        }

        var cableSpec = CableSpec.FromNullable(cableType, crossSection, cableColor, conductorCount);
        if (cableSpec is not null)
        {
            installation.UpdateCableSpec(cableSpec);
        }

        if (depth is not null)
        {
            installation.UpdateDepth(depth);
        }

        if (manufacturer is not null || modelName is not null || serialNumber is not null)
        {
            installation.UpdateDeviceInfo(manufacturer, modelName, serialNumber);
        }

        await installations.SaveAsync(installation, cancellationToken);
    }
}
