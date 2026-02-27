using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BauDoku.Documentation.Infrastructure.ReadModel;

public sealed class ReadModelDbContextFactory : IDesignTimeDbContextFactory<ReadModelDbContext>
{
    public ReadModelDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ReadModelDbContext>()
            .UseNpgsql("Host=localhost;Database=baudoku_documentation;Username=postgres;Password=postgres",
                o => o.UseNetTopologySuite())
            .Options;

        return new ReadModelDbContext(options);
    }
}
