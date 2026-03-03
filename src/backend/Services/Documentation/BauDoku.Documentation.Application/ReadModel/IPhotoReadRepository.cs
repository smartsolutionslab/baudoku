using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.ReadModel;

public interface IPhotoReadRepository
{
    Task<PhotoDto> GetByIdAsync(PhotoIdentifier photoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default);
}
