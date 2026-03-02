using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Commands.Handlers;

public sealed class UpdateInstallationCommandHandler(IInstallationRepository installations)
    : ICommandHandler<UpdateInstallationCommand>
{
    public async Task Handle(UpdateInstallationCommand command, CancellationToken cancellationToken = default)
    {
        var installation = await installations.GetByIdAsync(command.InstallationId, cancellationToken);

        if (command.Position is not null)
        {
            installation.UpdatePosition(command.Position);
        }

        if (command.Description is not null)
        {
            installation.UpdateDescription(command.Description);
        }

        if (command.CableType is not null)
        {
            installation.UpdateCableSpec(CableSpec.Create(command.CableType, command.CrossSection, command.CableColor, command.ConductorCount));
        }

        if (command.Depth is not null)
        {
            installation.UpdateDepth(command.Depth);
        }

        if (command.Manufacturer is not null || command.ModelName is not null || command.SerialNumber is not null)
        {
            installation.UpdateDeviceInfo(command.Manufacturer, command.ModelName, command.SerialNumber);
        }

        await installations.SaveAsync(installation, cancellationToken);
    }
}
