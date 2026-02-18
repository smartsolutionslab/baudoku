using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class PhotoReadRepository(DocumentationDbContext context) : IPhotoReadRepository
{
    public async Task<PhotoDto?> GetByIdAsync(PhotoIdentifier photoId, CancellationToken cancellationToken = default)
    {
        return await context.Photos
            .AsNoTracking()
            .Where(p => p.Id.Value == photoId.Value)
            .Select(p => new PhotoDto(
                p.Id.Value,
                EF.Property<Domain.ValueObjects.InstallationIdentifier>(p, "InstallationId").Value,
                p.FileName.Value,
                p.BlobUrl.Value,
                p.ContentType.Value,
                p.FileSize.Value,
                p.PhotoType.Value,
                p.Caption != null ? p.Caption.Value : null,
                p.Description != null ? p.Description.Value : null,
                p.Position != null ? p.Position.Latitude : null,
                p.Position != null ? p.Position.Longitude : null,
                p.Position != null ? p.Position.Altitude : null,
                p.Position != null ? (double?)p.Position.HorizontalAccuracy : null,
                p.Position != null ? p.Position.Source.Value : null,
                p.Position != null ? (p.Position.CorrectionService != null ? p.Position.CorrectionService.Value : null) : null,
                p.Position != null ? (p.Position.RtkFixStatus != null ? p.Position.RtkFixStatus.Value : null) : null,
                p.Position != null ? p.Position.SatelliteCount : null,
                p.Position != null ? p.Position.Hdop : null,
                p.Position != null ? p.Position.CorrectionAge : null,
                p.TakenAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PhotoDto>> ListByInstallationIdAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default)
    {
        return await context.Photos
            .AsNoTracking()
            .Where(p => EF.Property<InstallationIdentifier>(p, "InstallationId").Value == installationId.Value)
            .OrderByDescending(p => p.TakenAt)
            .Select(p => new PhotoDto(
                p.Id.Value,
                installationId.Value,
                p.FileName.Value,
                p.BlobUrl.Value,
                p.ContentType.Value,
                p.FileSize.Value,
                p.PhotoType.Value,
                p.Caption != null ? p.Caption.Value : null,
                p.Description != null ? p.Description.Value : null,
                p.Position != null ? p.Position.Latitude : null,
                p.Position != null ? p.Position.Longitude : null,
                p.Position != null ? p.Position.Altitude : null,
                p.Position != null ? (double?)p.Position.HorizontalAccuracy : null,
                p.Position != null ? p.Position.Source.Value : null,
                p.Position != null ? (p.Position.CorrectionService != null ? p.Position.CorrectionService.Value : null) : null,
                p.Position != null ? (p.Position.RtkFixStatus != null ? p.Position.RtkFixStatus.Value : null) : null,
                p.Position != null ? p.Position.SatelliteCount : null,
                p.Position != null ? p.Position.Hdop : null,
                p.Position != null ? p.Position.CorrectionAge : null,
                p.TakenAt))
            .ToListAsync(cancellationToken);
    }
}
