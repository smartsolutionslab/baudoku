using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Infrastructure.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class PhotoReadRepository(ReadModelDbContext context) : IPhotoReadRepository
{
    public async Task<PhotoDto> GetByIdAsync(PhotoIdentifier photoId, CancellationToken cancellationToken = default)
    {
        var id = photoId.Value;
        return await context.Photos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .SelectPhotoDtos()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Foto mit ID '{id}' nicht gefunden.");
    }

    public async Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default)
    {
        return await context.Photos
            .AsNoTracking()
            .Where(p => p.InstallationId == installationId.Value)
            .OrderByDescending(p => p.TakenAt)
            .SelectPhotoDtos()
            .ToListAsync(cancellationToken);
    }
}
