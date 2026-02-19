using BauDoku.Documentation.Domain;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.Persistence.Repositories;

public sealed class InstallationRepository(DocumentationDbContext context) : IInstallationRepository
{
    public async Task<Installation> GetByIdAsync(InstallationIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Installations
            .Include(i => i.Photos)
            .Include(i => i.Measurements)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Installation mit ID '{id.Value}' nicht gefunden.");
    }

    public async Task<Installation> GetByIdReadOnlyAsync(InstallationIdentifier id, CancellationToken cancellationToken = default)
    {
        return await context.Installations
            .AsNoTracking()
            .Include(i => i.Photos)
            .Include(i => i.Measurements)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Installation mit ID '{id.Value}' nicht gefunden.");
    }

    public async Task AddAsync(Installation aggregate, CancellationToken cancellationToken = default)
    {
        await context.Installations.AddAsync(aggregate, cancellationToken);
    }

    public void Remove(Installation installation)
    {
        context.Installations.Remove(installation);
    }
}
