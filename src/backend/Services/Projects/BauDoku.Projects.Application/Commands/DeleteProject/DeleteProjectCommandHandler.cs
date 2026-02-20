using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(IProjectRepository projects, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await projects.GetByIdAsync(command.ProjectId, cancellationToken);

        project.Delete();
        projects.Remove(project);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ProjectsDeleted.Add(1);
    }
}
