using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.Rules;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(IProjectRepository projects, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var (name, street, city, zipCode, clientName, clientEmail, clientPhone) = command;

        var projectId = ProjectIdentifier.New();
        var projectName = ProjectName.From(name);

        var nameExists = await projects.ExistsByNameAsync(projectName, cancellationToken);
        var rule = new ProjectMustHaveUniqueName(nameExists);
        if (rule.IsBroken()) throw new BusinessRuleException(rule);

        var address = Address.Create(Street.From(street), City.From(city), ZipCode.From(zipCode));
        var client = ClientInfo.Create(clientName, clientEmail, clientPhone);

        var project = Project.Create(projectId, projectName, address, client);

        await projects.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ProjectsCreated.Add(1);

        return projectId.Value;
    }
}
