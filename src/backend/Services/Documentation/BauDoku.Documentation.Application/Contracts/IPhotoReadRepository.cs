using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Contracts;

public interface IPhotoReadRepository
{
    Task<PhotoDto?> GetByIdAsync(Guid photoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(Guid installationId, CancellationToken cancellationToken = default);
}
