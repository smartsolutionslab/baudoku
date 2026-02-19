using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain;

public interface IProjectRepository : IRepository<Project, ProjectIdentifier>
{
    Task<bool> ExistsByNameAsync(ProjectName name, CancellationToken ct = default);
    void Remove(Project project);
}
