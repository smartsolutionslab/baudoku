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
            .HasConversion(id => id.Value, value => PhotoIdentifier.From(value));

        builder.Property(p => p.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(FileName.MaxLength)
            .IsRequired()
            .HasConversion(fn => fn.Value, value => FileName.From(value));

        builder.Property(p => p.BlobUrl)
            .HasColumnName("blob_url")
            .HasMaxLength(BlobUrl.MaxLength)
            .IsRequired()
            .HasConversion(bu => bu.Value, value => BlobUrl.From(value));

        builder.Property(p => p.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(ContentType.MaxLength)
            .IsRequired()
            .HasConversion(ct => ct.Value, value => ContentType.From(value));

        builder.Property(p => p.FileSize)
            .HasColumnName("file_size")
            .IsRequired()
            .HasConversion(fs => fs.Value, value => FileSize.From(value));

        builder.Property(p => p.PhotoType)
            .HasColumnName("photo_type")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(pt => pt.Value, value => PhotoType.From(value));

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
            pos.Property(p => p.Latitude).HasColumnName("gps_latitude")
                .HasConversion(v => v.Value, v => Latitude.From(v));
            pos.Property(p => p.Longitude).HasColumnName("gps_longitude")
                .HasConversion(v => v.Value, v => Longitude.From(v));
            pos.Property(p => p.Altitude).HasColumnName("gps_altitude");
            pos.Property(p => p.HorizontalAccuracy).HasColumnName("gps_horizontal_accuracy")
                .HasConversion(v => v.Value, v => HorizontalAccuracy.From(v));
            pos.Property(p => p.Source).HasColumnName("gps_source").HasMaxLength(GpsSource.MaxLength)
                .HasConversion(s => s.Value, v => GpsSource.From(v));
            pos.Property(p => p.CorrectionService).HasColumnName("gps_correction_service").HasMaxLength(CorrectionService.MaxLength)
                .HasConversion(
                    s => s != null ? s.Value : null,
                    v => v != null ? CorrectionService.From(v) : null);
            pos.Property(p => p.RtkFixStatus).HasColumnName("gps_rtk_fix_status").HasMaxLength(RtkFixStatus.MaxLength)
                .HasConversion(
                    s => s != null ? s.Value : null,
                    v => v != null ? RtkFixStatus.From(v) : null);
            pos.Property(p => p.SatelliteCount).HasColumnName("gps_satellite_count");
            pos.Property(p => p.Hdop).HasColumnName("gps_hdop");
            pos.Property(p => p.CorrectionAge).HasColumnName("gps_correction_age");
        });

        builder.Property<InstallationIdentifier>("InstallationId")
            .HasColumnName("installation_id")
            .HasConversion(id => id.Value, value => InstallationIdentifier.From(value));
    }
}
