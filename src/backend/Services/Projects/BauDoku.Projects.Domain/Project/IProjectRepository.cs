using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public interface IProjectRepository : IRepository<Project, ProjectIdentifier>
{
    Task<bool> ExistsByNameAsync(ProjectName name, CancellationToken cancellationToken = default);
    void Remove(Project project);
}
