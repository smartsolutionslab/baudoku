using BauDoku.Documentation.Domain.Entities;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Documentation.Infrastructure.Persistence.Configurations;

public sealed class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable("photos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => new PhotoId(value));

        builder.Property(p => p.FilePath)
            .HasColumnName("file_path")
            .HasMaxLength(500)
            .IsRequired();

        builder.OwnsOne(p => p.Description, desc =>
        {
            desc.Property(d => d.Value).HasColumnName("description").HasMaxLength(Description.MaxLength);
        });

        builder.Property(p => p.CapturedAt)
            .HasColumnName("captured_at")
            .IsRequired();

        builder.OwnsOne(p => p.Position, pos =>
        {
            pos.Property(p => p.Latitude).HasColumnName("gps_latitude");
            pos.Property(p => p.Longitude).HasColumnName("gps_longitude");
            pos.Property(p => p.Altitude).HasColumnName("gps_altitude");
            pos.Property(p => p.HorizontalAccuracy).HasColumnName("gps_horizontal_accuracy");
            pos.Property(p => p.Source).HasColumnName("gps_source").HasMaxLength(20);
            pos.Property(p => p.CorrectionService).HasColumnName("gps_correction_service").HasMaxLength(20);
            pos.Property(p => p.RtkFixStatus).HasColumnName("gps_rtk_fix_status").HasMaxLength(10);
            pos.Property(p => p.SatelliteCount).HasColumnName("gps_satellite_count");
            pos.Property(p => p.Hdop).HasColumnName("gps_hdop");
            pos.Property(p => p.CorrectionAge).HasColumnName("gps_correction_age");
        });

        builder.Property<InstallationId>("InstallationId")
            .HasColumnName("installation_id")
            .HasConversion(id => id.Value, value => new InstallationId(value));
    }
}
