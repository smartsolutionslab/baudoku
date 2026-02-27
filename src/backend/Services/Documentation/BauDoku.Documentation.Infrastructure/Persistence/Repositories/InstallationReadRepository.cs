using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.BuildingBlocks.Infrastructure.Pagination;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Infrastructure.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationReadRepository(ReadModelDbContext context) : IInstallationReadRepository
{
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
        var (latitude, longitude, radiusMeters) = radius;

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
        var (minLatitude, minLongitude, maxLatitude, maxLongitude) = boundingBox;

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
