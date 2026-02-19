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
        var (projectId, name, type, parentZoneId) = command;
        var project = await projects.GetByIdAsync(ProjectIdentifier.From(projectId), cancellationToken);

        var zoneId = ZoneIdentifier.New();
        var zoneName = ZoneName.From(name);
        var zoneType = ZoneType.From(type);
        var parentZoneIdentifier = ZoneIdentifier.FromNullable(parentZoneId);

        project.AddZone(zoneId, zoneName, zoneType, parentZoneIdentifier);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ZonesAdded.Add(1);
        ProjectsMetrics.ZonesPerProject.Record(project.Zones.Count);
    }
}
