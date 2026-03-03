using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.ReadModel;

public interface IProjectReadRepository : IPagedReadRepository<ProjectListItemDto, SearchTerm?>
{
}
