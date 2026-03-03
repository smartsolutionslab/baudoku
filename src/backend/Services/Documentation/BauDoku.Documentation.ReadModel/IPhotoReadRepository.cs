using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.ReadModel;

public interface IPhotoReadRepository : IReadRepository<PhotoDto, PhotoIdentifier>
{
    Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default);
}
