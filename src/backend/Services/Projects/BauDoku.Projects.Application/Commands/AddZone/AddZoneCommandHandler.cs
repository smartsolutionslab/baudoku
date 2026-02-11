using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Commands.AddZone;

public sealed class AddZoneCommandHandler : ICommandHandler<AddZoneCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddZoneCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddZoneCommand command, CancellationToken cancellationToken = default)
    {
        var projectId = new ProjectId(command.ProjectId);
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken)
            ?? throw new InvalidOperationException($"Projekt mit ID '{command.ProjectId}' wurde nicht gefunden.");

        var zoneId = ZoneId.New();
        var zoneName = new ZoneName(command.Name);
        var zoneType = new ZoneType(command.Type);
        var parentZoneId = command.ParentZoneId.HasValue ? new ZoneId(command.ParentZoneId.Value) : null;

        project.AddZone(zoneId, zoneName, zoneType, parentZoneId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ZonesAdded.Add(1);
    }
}
