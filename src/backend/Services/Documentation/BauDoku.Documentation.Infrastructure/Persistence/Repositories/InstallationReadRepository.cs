using System.Linq.Expressions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationReadRepository(DocumentationDbContext context) : IInstallationReadRepository
{
    private static readonly Expression<Func<Installation, InstallationListItemDto>> toInstallationListItem = i => new InstallationListItemDto(
        i.Id.Value,
        i.ProjectId.Value,
        i.Type.Value,
        i.Status.Value,
        i.QualityGrade.Value,
        i.Position.Latitude.Value,
        i.Position.Longitude.Value,
        i.Description != null ? i.Description.Value : null,
        i.CreatedAt,
        i.Photos.Count,
        i.Measurements.Count);

    public async Task<PagedResult<InstallationListItemDto>> ListAsync(InstallationListFilter filter, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var (projectId, zoneId, type, status, search) = filter;
        var (page, pageSize) = pagination;

        var query = context.Installations.AsNoTracking();

        if (projectId is not null)
            query = query.Where(i => i.ProjectId.Value == projectId.Value);

        if (zoneId is not null)
            query = query.Where(i => i.ZoneId != null && i.ZoneId.Value == zoneId.Value);

        if (type is not null)
            query = query.Where(i => i.Type.Value == type.Value);

        if (status is not null)
            query = query.Where(i => i.Status.Value == status.Value);

        if (search.HasValue())
            query = query.Where(i => i.Description != null && EF.Functions.ILike(i.Description.Value, $"%{search}%"));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(toInstallationListItem)
            .ToListAsync(cancellationToken);

        return new PagedResult<InstallationListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<PagedResult<NearbyInstallationDto>> SearchInRadiusAsync(
        SearchRadius radius,
        ProjectIdentifier? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var (latitude, longitude, radiusMeters) = radius;
        var (page, pageSize) = pagination;

        var baseQuery = context.Database.SqlQuery<NearbyInstallationDto>(
            $"""
            SELECT
                i.id AS "Id",
                i.project_id AS "ProjectId",
                i.type AS "Type",
                i.status AS "Status",
                i.gps_quality_grade AS "QualityGrade",
                i.gps_latitude AS "Latitude",
                i.gps_longitude AS "Longitude",
                i.description AS "Description",
                i.created_at AS "CreatedAt",
                (SELECT COUNT(*)::int FROM photos p WHERE p.installation_id = i.id) AS "PhotoCount",
                (SELECT COUNT(*)::int FROM measurements m WHERE m.installation_id = i.id) AS "MeasurementCount",
                ST_Distance(
                    i.location::geography,
                    ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326)::geography
                ) AS "DistanceMeters"
            FROM installations i
            WHERE ST_DWithin(
                i.location::geography,
                ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326)::geography,
                {radiusMeters})
            """);

        if (projectId is not null)
            baseQuery = baseQuery.Where(x => x.ProjectId == projectId.Value);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderBy(x => x.DistanceMeters)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<NearbyInstallationDto>(items, totalCount, page, pageSize);
    }

    public async Task<PagedResult<InstallationListItemDto>> SearchInBoundingBoxAsync(
        BoundingBox boundingBox,
        ProjectIdentifier? projectId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var (minLatitude, minLongitude, maxLatitude, maxLongitude) = boundingBox;
        var (page, pageSize) = pagination;

        var query = context.Installations
            .FromSqlInterpolated(
                $"""
                SELECT * FROM installations
                WHERE ST_Within(
                    location,
                    ST_MakeEnvelope({minLongitude}, {minLatitude}, {maxLongitude}, {maxLatitude}, 4326))
                """)
            .AsNoTracking();

        if (projectId is not null)
            query = query.Where(i => i.ProjectId.Value == projectId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(toInstallationListItem)
            .ToListAsync(cancellationToken);

        return new PagedResult<InstallationListItemDto>(items, totalCount, page, pageSize);
    }
}
