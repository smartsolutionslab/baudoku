using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(IProjectRepository projects, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        var projectId = ProjectIdentifier.From(command.ProjectId);
        var project = await projects.GetByIdAsync(projectId, cancellationToken)
            ?? throw new KeyNotFoundException($"Projekt mit ID '{command.ProjectId}' wurde nicht gefunden.");

        project.Delete();
        projects.Remove(project);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ProjectsDeleted.Add(1);
    }
}
