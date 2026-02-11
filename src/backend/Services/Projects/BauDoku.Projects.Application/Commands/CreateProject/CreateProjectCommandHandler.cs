using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Diagnostics;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Commands.CreateProject;

public sealed class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, Guid>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var projectId = ProjectId.New();
        var name = new ProjectName(command.Name);
        var address = new Address(command.Street, command.City, command.ZipCode);
        var client = new ClientInfo(command.ClientName, command.ClientEmail, command.ClientPhone);

        var project = Project.Create(projectId, name, address, client);

        await _projectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        ProjectsMetrics.ProjectsCreated.Add(1);

        return projectId.Value;
    }
}
