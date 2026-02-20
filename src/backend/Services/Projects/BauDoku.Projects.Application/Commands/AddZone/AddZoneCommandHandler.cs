using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Commands.AddZone;

public sealed class AddZoneCommandHandler(IProjectRepository projects, IUnitOfWork unitOfWork)
    : ICommandHandler<AddZoneCommand>
{
    public async Task Handle(AddZoneCommand command, CancellationToken cancellationToken = default)
    {
        var project = await projects.GetByIdAsync(command.ProjectId, cancellationToken);

        var zoneId = ZoneIdentifier.New();
        project.AddZone(zoneId, command.Name, command.Type, command.ParentZoneId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ZonesAdded.Add(1);
        ProjectsMetrics.ZonesPerProject.Record(project.Zones.Count);
    }
}
