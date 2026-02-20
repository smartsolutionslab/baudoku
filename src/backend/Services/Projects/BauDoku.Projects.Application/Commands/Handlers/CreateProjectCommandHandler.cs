using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Commands.Handlers;

public sealed class CreateProjectCommandHandler(IProjectRepository projects, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProjectCommand, ProjectIdentifier>
{
    public async Task<ProjectIdentifier> Handle(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var (name, street, city, zipCode, clientName, clientEmail, clientPhone) = command;

        var nameExists = await projects.ExistsByNameAsync(name, cancellationToken);
        var rule = new ProjectMustHaveUniqueName(nameExists);
        if (rule.IsBroken()) throw new BusinessRuleException(rule);

        var projectId = ProjectIdentifier.New();
        var address = Address.Create(street, city, zipCode);
        var client = ClientInfo.Create(clientName, clientEmail, clientPhone);

        var project = Project.Create(projectId, name, address, client);

        await projects.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ProjectsCreated.Add(1);

        return projectId;
    }
}
