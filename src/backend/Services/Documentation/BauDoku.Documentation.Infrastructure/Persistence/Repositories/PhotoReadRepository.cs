using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class PhotoReadRepository : IPhotoReadRepository
{
    private readonly DocumentationDbContext _context;

    public PhotoReadRepository(DocumentationDbContext context)
    {
        _context = context;
    }

    public async Task<PhotoDto?> GetByIdAsync(Guid photoId, CancellationToken cancellationToken = default)
    {
        return await _context.Photos
            .AsNoTracking()
            .Where(p => p.Id.Value == photoId)
            .Select(p => new PhotoDto(
                p.Id.Value,
                EF.Property<Domain.ValueObjects.InstallationId>(p, "InstallationId").Value,
                p.FileName,
                p.BlobUrl,
                p.ContentType,
                p.FileSize,
                p.PhotoType.Value,
                p.Caption != null ? p.Caption.Value : null,
                p.Description != null ? p.Description.Value : null,
                p.Position != null ? p.Position.Latitude : null,
                p.Position != null ? p.Position.Longitude : null,
                p.TakenAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(Guid installationId, CancellationToken cancellationToken = default)
    {
        return await _context.Photos
            .AsNoTracking()
            .Where(p => EF.Property<Domain.ValueObjects.InstallationId>(p, "InstallationId").Value == installationId)
            .OrderByDescending(p => p.TakenAt)
            .Select(p => new PhotoDto(
                p.Id.Value,
                installationId,
                p.FileName,
                p.BlobUrl,
                p.ContentType,
                p.FileSize,
                p.PhotoType.Value,
                p.Caption != null ? p.Caption.Value : null,
                p.Description != null ? p.Description.Value : null,
                p.Position != null ? p.Position.Latitude : null,
                p.Position != null ? p.Position.Longitude : null,
                p.TakenAt))
            .ToListAsync(cancellationToken);
    }
}
