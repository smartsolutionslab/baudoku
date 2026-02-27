using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public interface IInstallationRepository : IEventSourcedRepository<Installation, InstallationIdentifier>
{
}
