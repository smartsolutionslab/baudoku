using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Commands.AddZone;

public sealed class AddZoneCommandHandler(IProjectRepository projects, IUnitOfWork unitOfWork)
    : ICommandHandler<AddZoneCommand>
{
    public async Task Handle(AddZoneCommand command, CancellationToken cancellationToken = default)
    {
        var projectId = ProjectIdentifier.From(command.ProjectId);
        var project = await projects.GetByIdAsync(projectId, cancellationToken)
            ?? throw new InvalidOperationException($"Projekt mit ID '{command.ProjectId}' wurde nicht gefunden.");

        var zoneId = ZoneIdentifier.New();
        var zoneName = ZoneName.From(command.Name);
        var zoneType = ZoneType.From(command.Type);
        var parentZoneId = command.ParentZoneId.HasValue ? ZoneIdentifier.From(command.ParentZoneId.Value) : null;

        project.AddZone(zoneId, zoneName, zoneType, parentZoneId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ZonesAdded.Add(1);
        ProjectsMetrics.ZonesPerProject.Record(project.Zones.Count);
    }
}
