using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class PhotoReadRepository(ReadModelDbContext context) : IPhotoReadRepository
{
    private readonly DbSet<PhotoReadModel> photos = context.Photos;

    public async Task<PhotoDto> GetByIdAsync(PhotoIdentifier photoId, CancellationToken cancellationToken = default)
    {
        var id = photoId.Value;
        return (await photos
            .Where(p => p.Id == id)
            .SelectPhotoDtos()
            .FirstOrDefaultAsync(cancellationToken))
            .OrNotFound("Foto", id);
    }

    public async Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default)
    {
        return await photos
            .Where(p => p.InstallationId == installationId.Value)
            .OrderByDescending(p => p.TakenAt)
            .SelectPhotoDtos()
            .ToListAsync(cancellationToken);
    }
}
