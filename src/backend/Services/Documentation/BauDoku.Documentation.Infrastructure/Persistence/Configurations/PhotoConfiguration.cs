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

        builder.Property(p => p.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.BlobUrl)
            .HasColumnName("blob_url")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.FileSize)
            .HasColumnName("file_size")
            .IsRequired();

        builder.Property(p => p.PhotoType)
            .HasColumnName("photo_type")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(pt => pt.Value, value => new PhotoType(value));

        builder.OwnsOne(p => p.Caption, cap =>
        {
            cap.Property(c => c.Value).HasColumnName("caption").HasMaxLength(Caption.MaxLength);
        });

        builder.OwnsOne(p => p.Description, desc =>
        {
            desc.Property(d => d.Value).HasColumnName("description").HasMaxLength(Description.MaxLength);
        });

        builder.Property(p => p.TakenAt)
            .HasColumnName("taken_at")
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
