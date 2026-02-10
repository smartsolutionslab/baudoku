using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Queries.GetProject;

public sealed class GetProjectQueryHandler : IQueryHandler<GetProjectQuery, ProjectDto?>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDto?> Handle(GetProjectQuery query, CancellationToken cancellationToken = default)
    {
        var projectId = new ProjectId(query.ProjectId);
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project is null)
            return null;

        var zones = project.Zones.Select(z => new ZoneDto(
            z.Id.Value,
            z.Name.Value,
            z.Type.Value,
            z.ParentZoneId?.Value)).ToList();

        return new ProjectDto(
            project.Id.Value,
            project.Name.Value,
            project.Status.Value,
            project.Address.Street,
            project.Address.City,
            project.Address.ZipCode,
            project.Client.Name,
            project.Client.Email,
            project.Client.Phone,
            project.CreatedAt,
            zones);
    }
}
