using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Contracts;

public interface IPhotoReadRepository
{
    Task<PhotoDto?> GetByIdAsync(PhotoIdentifier photoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default);
}
