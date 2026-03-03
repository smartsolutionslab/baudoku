using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public interface IInstallationRepository : IEventSourcedRepository<Installation, InstallationIdentifier>
{
}
