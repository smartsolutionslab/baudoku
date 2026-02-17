using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Application.Contracts;

public interface IProjectRepository : IRepository<Project, ProjectIdentifier>
{
    Task<bool> ExistsByNameAsync(ProjectName name, CancellationToken ct = default);
    void Remove(Project project);
}
