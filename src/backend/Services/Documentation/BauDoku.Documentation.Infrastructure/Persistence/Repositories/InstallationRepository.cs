using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationRepository : IInstallationRepository
{
    private readonly DocumentationDbContext context;

    public InstallationRepository(DocumentationDbContext context)
    {
        this.context = context;
    }

    public async Task<Installation?> GetByIdAsync(InstallationIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Installations
            .Include(i => i.Photos)
            .Include(i => i.Measurements)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task AddAsync(Installation aggregate, CancellationToken cancellationToken = default)
    {
        await context.Installations.AddAsync(aggregate, cancellationToken);
    }
}
