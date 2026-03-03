using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Persistence;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Projects.Application.Diagnostics;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Application.Commands.Handlers;

public sealed class AddZoneCommandHandler(IProjectRepository projects, IUnitOfWork unitOfWork)
    : ICommandHandler<AddZoneCommand>
{
    public async Task Handle(AddZoneCommand command, CancellationToken cancellationToken = default)
    {
        var (projectId, name, type, parentZoneId) = command;

        var project = await projects.With(projectId, cancellationToken);

        var zoneId = ZoneIdentifier.New();
        project.AddZone(zoneId, name, type, parentZoneId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ZonesAdded.Add(1);
        ProjectsMetrics.ZonesPerProject.Record(project.Zones.Count);
    }
}
