using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.ReadModel;
using BauDoku.BuildingBlocks.Persistence.Pagination;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Infrastructure.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationReadRepository(ReadModelDbContext context) : IInstallationReadRepository
{
    public async Task<InstallationDto> GetByIdAsync(InstallationIdentifier id, CancellationToken cancellationToken = default)
    {
        var installation = await context.Installations
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id.Value && !i.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Installation mit ID '{id.Value}' wurde nicht gefunden.");

        var photos = await context.Photos
            .AsNoTracking()
            .Where(p => p.InstallationId == id.Value)
            .OrderByDescending(p => p.TakenAt)
            .SelectPhotoDtos()
            .ToListAsync(cancellationToken);

        var measurements = await context.Measurements
            .AsNoTracking()
            .Where(m => m.InstallationId == id.Value)
            .OrderByDescending(m => m.MeasuredAt)
            .SelectMeasurementDtos()
            .ToListAsync(cancellationToken);

        return installation.ToInstallationDto(photos, measurements);
    }

    public async Task<IReadOnlyList<MeasurementDto>> GetMeasurementsAsync(InstallationIdentifier installationId, CancellationToken cancellationToken = default)
    {
        var exists = await context.Installations
            .AsNoTracking()
            .AnyAsync(i => i.Id == installationId.Value && !i.IsDeleted, cancellationToken);

        if (!exists) throw new KeyNotFoundException($"Installation mit ID '{installationId.Value}' wurde nicht gefunden.");

        return await context.Measurements
            .AsNoTracking()
            .Where(m => m.InstallationId == installationId.Value)
            .OrderByDescending(m => m.MeasuredAt)
            .SelectMeasurementDtos()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<InstallationListItemDto>> ListAsync(InstallationListFilter filter, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type, status, search) = filter;

        var query = context.Installations.AsNoTracking().Where(i => !i.IsDeleted);

        if (projectId is not null)
            query = query.Where(i => i.ProjectId == projectId.Value);

        if (zoneId is not null)
            query = query.Where(i => i.ZoneId == zoneId.Value);

        if (type is not null)
            query = query.Where(i => i.Type == type.Value);

        if (status is not null)
            query = query.Where(i => i.Status == status.Value);

        if (search is not null)
            query = query.Where(i => i.Description != null && EF.Functions.ILike(i.Description, $"%{search.Value}%"));

        return await query
            .OrderByDescending(i => i.CreatedAt)
            .SelectListItems()
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<PagedResult<NearbyInstallationDto>> SearchInRadiusAsync(
        SearchRadius radius,
        ProjectIdentifier? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var latitude = radius.Latitude.Value;
        var longitude = radius.Longitude.Value;
        var radiusMeters = radius.RadiusMeters.Value;

        var baseQuery = context.Database.SqlQuery<NearbyInstallationDto>(
            $"""
            SELECT
                i.id AS "Id",
                i.project_id AS "ProjectId",
                i.type AS "Type",
                i.status AS "Status",
                i.quality_grade AS "QualityGrade",
                i.latitude AS "Latitude",
                i.longitude AS "Longitude",
                i.description AS "Description",
                i.created_at AS "CreatedAt",
                i.photo_count AS "PhotoCount",
                i.measurement_count AS "MeasurementCount",
                ST_Distance(
                    ST_SetSRID(ST_MakePoint(i.longitude, i.latitude), 4326)::geography,
                    ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326)::geography
                ) AS "DistanceMeters"
            FROM documentation_read.installations i
            WHERE i.is_deleted = false
            AND ST_DWithin(
                ST_SetSRID(ST_MakePoint(i.longitude, i.latitude), 4326)::geography,
                ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326)::geography,
                {radiusMeters})
            """);

        if (projectId is not null)
            baseQuery = baseQuery.Where(x => x.ProjectId == projectId.Value);

        return await baseQuery
            .OrderBy(x => x.DistanceMeters)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<PagedResult<InstallationListItemDto>> SearchInBoundingBoxAsync(
        BoundingBox boundingBox,
        ProjectIdentifier? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var minLatitude = boundingBox.MinLatitude.Value;
        var minLongitude = boundingBox.MinLongitude.Value;
        var maxLatitude = boundingBox.MaxLatitude.Value;
        var maxLongitude = boundingBox.MaxLongitude.Value;

        var query = context.Installations
            .FromSqlInterpolated(
                $"""
                SELECT * FROM documentation_read.installations
                WHERE is_deleted = false
                AND ST_Within(
                    ST_SetSRID(ST_MakePoint(longitude, latitude), 4326),
                    ST_MakeEnvelope({minLongitude}, {minLatitude}, {maxLongitude}, {maxLatitude}, 4326))
                """)
            .AsNoTracking();

        if (projectId is not null)
            query = query.Where(i => i.ProjectId == projectId.Value);

        return await query
            .OrderByDescending(i => i.CreatedAt)
            .SelectListItems()
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
