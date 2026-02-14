using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.Rules;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Commands.CreateProject;

public sealed class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, Guid>
{
    private readonly IProjectRepository projectRepository;
    private readonly IUnitOfWork unitOfWork;

    public CreateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        this.projectRepository = projectRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var projectId = ProjectIdentifier.New();
        var name = ProjectName.From(command.Name);

        var nameExists = await projectRepository.ExistsByNameAsync(name, cancellationToken);
        var rule = new ProjectMustHaveUniqueName(nameExists);
        if (rule.IsBroken())
            throw new BusinessRuleException(rule);

        var address = Address.Create(command.Street, command.City, command.ZipCode);
        var client = ClientInfo.Create(command.ClientName, command.ClientEmail, command.ClientPhone);

        var project = Project.Create(projectId, name, address, client);

        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ProjectsCreated.Add(1);

        return projectId.Value;
    }
}
