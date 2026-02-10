using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationRepository : IInstallationRepository
{
    private readonly DocumentationDbContext _context;

    public InstallationRepository(DocumentationDbContext context)
    {
        _context = context;
    }

    public async Task<Installation?> GetByIdAsync(InstallationId id, CancellationToken cancellationToken = default)
    {
        return await _context.Installations
            .Include(i => i.Photos)
            .Include(i => i.Measurements)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task AddAsync(Installation aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Installations.AddAsync(aggregate, cancellationToken);
    }
}
