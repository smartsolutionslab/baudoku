using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public interface IInstallationRepository : IRepository<Installation, InstallationIdentifier>
{
    void Remove(Installation installation);
}
