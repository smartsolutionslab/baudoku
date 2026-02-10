using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationReadRepository : IInstallationReadRepository
{
    private readonly DocumentationDbContext _context;

    public InstallationReadRepository(DocumentationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<InstallationListItemDto>> ListAsync(
        Guid? projectId,
        Guid? zoneId,
        string? type,
        string? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Installations.AsNoTracking();

        if (projectId.HasValue)
            query = query.Where(i => i.ProjectId == projectId.Value);

        if (zoneId.HasValue)
            query = query.Where(i => i.ZoneId == zoneId.Value);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(i => EF.Functions.ILike(i.Type.Value, type));

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(i => EF.Functions.ILike(i.Status.Value, status));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i =>
                i.Description != null && EF.Functions.ILike(i.Description.Value, $"%{search}%"));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new InstallationListItemDto(
                i.Id.Value,
                i.ProjectId,
                i.Type.Value,
                i.Status.Value,
                i.Position.Latitude,
                i.Position.Longitude,
                i.Description != null ? i.Description.Value : null,
                i.CreatedAt,
                i.Photos.Count,
                i.Measurements.Count))
            .ToListAsync(cancellationToken);

        return new PagedResult<InstallationListItemDto>(items, totalCount, page, pageSize);
    }
}
