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
            .Select(p => new PhotoDto(
                p.Id,
                p.InstallationId,
                p.FileName,
                p.BlobUrl,
                p.ContentType,
                p.FileSize,
                p.PhotoType,
                p.Caption,
                p.Description,
                p.Latitude,
                p.Longitude,
                p.Altitude,
                p.HorizontalAccuracy,
                p.GpsSource,
                p.CorrectionService,
                p.RtkFixStatus,
                p.SatelliteCount,
                p.Hdop,
                p.CorrectionAge,
                p.TakenAt))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Foto mit ID '{id}' nicht gefunden.");
    }

    public async Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default)
    {
        return await context.Photos
            .AsNoTracking()
            .Where(p => p.InstallationId == installationId.Value)
            .OrderByDescending(p => p.TakenAt)
            .Select(p => new PhotoDto(
                p.Id,
                p.InstallationId,
                p.FileName,
                p.BlobUrl,
                p.ContentType,
                p.FileSize,
                p.PhotoType,
                p.Caption,
                p.Description,
                p.Latitude,
                p.Longitude,
                p.Altitude,
                p.HorizontalAccuracy,
                p.GpsSource,
                p.CorrectionService,
                p.RtkFixStatus,
                p.SatelliteCount,
                p.Hdop,
                p.CorrectionAge,
                p.TakenAt))
            .ToListAsync(cancellationToken);
    }
}
