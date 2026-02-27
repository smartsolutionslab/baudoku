using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.Infrastructure.ReadModel;

public sealed class ReadModelDbContext(DbContextOptions<ReadModelDbContext> options) : DbContext(options)
{
    public DbSet<InstallationReadModel> Installations => Set<InstallationReadModel>();
    public DbSet<PhotoReadModel> Photos => Set<PhotoReadModel>();
    public DbSet<MeasurementReadModel> Measurements => Set<MeasurementReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("documentation_read");
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<InstallationReadModel>(entity =>
        {
            entity.ToTable("installations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.ZoneId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasMany(e => e.Photos)
                .WithOne(p => p.Installation)
                .HasForeignKey(p => p.InstallationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Measurements)
                .WithOne(m => m.Installation)
                .HasForeignKey(m => m.InstallationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PhotoReadModel>(entity =>
        {
            entity.ToTable("photos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<MeasurementReadModel>(entity =>
        {
            entity.ToTable("measurements");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}
