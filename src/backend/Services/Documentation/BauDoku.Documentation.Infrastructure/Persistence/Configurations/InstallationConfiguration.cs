using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.Entities;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace BauDoku.Documentation.Infrastructure.Persistence.Configurations;

public sealed class InstallationConfiguration : IEntityTypeConfiguration<Installation>
{
    public void Configure(EntityTypeBuilder<Installation> builder)
    {
        builder.ToTable("installations");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => InstallationIdentifier.From(value));

        builder.Property(i => i.ProjectId)
            .HasColumnName("project_id")
            .IsRequired()
            .HasConversion(id => id.Value, value => ProjectIdentifier.From(value));

        builder.Property(i => i.ZoneId)
            .HasColumnName("zone_id")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? ZoneIdentifier.From(value.Value) : null);

        builder.Property(i => i.Type)
            .HasColumnName("type")
            .HasMaxLength(30)
            .IsRequired()
            .HasConversion(t => t.Value, value => InstallationType.From(value));

        builder.Property(i => i.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(s => s.Value, value => InstallationStatus.From(value));

        builder.Property(i => i.QualityGrade)
            .HasColumnName("gps_quality_grade")
            .HasMaxLength(1)
            .IsRequired()
            .HasConversion(g => g.Value, value => GpsQualityGrade.From(value));

        builder.OwnsOne(i => i.Position, pos =>
        {
            pos.Property(p => p.Latitude).HasColumnName("gps_latitude").IsRequired()
                .HasConversion(v => v.Value, v => Latitude.From(v));
            pos.Property(p => p.Longitude).HasColumnName("gps_longitude").IsRequired()
                .HasConversion(v => v.Value, v => Longitude.From(v));
            pos.Property(p => p.Altitude).HasColumnName("gps_altitude");
            pos.Property(p => p.HorizontalAccuracy).HasColumnName("gps_horizontal_accuracy").IsRequired()
                .HasConversion(v => v.Value, v => HorizontalAccuracy.From(v));
            pos.Property(p => p.Source).HasColumnName("gps_source").HasMaxLength(GpsSource.MaxLength).IsRequired()
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

        builder.Navigation(i => i.Position).IsRequired();

        // PostGIS computed geometry column for spatial queries (BD-801)
        builder.Property<Point>("Location")
            .HasColumnName("location")
            .HasColumnType("geometry(Point, 4326)")
            .HasComputedColumnSql("ST_SetSRID(ST_MakePoint(gps_longitude, gps_latitude), 4326)", stored: true)
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex("Location")
            .HasMethod("gist");

        builder.OwnsOne(i => i.Description, desc =>
        {
            desc.Property(d => d.Value).HasColumnName("description").HasMaxLength(Description.MaxLength);
        });

        builder.OwnsOne(i => i.CableSpec, cable =>
        {
            cable.Property(c => c.CableType).HasColumnName("cable_type").HasMaxLength(CableType.MaxLength)
                .HasConversion(ct => ct.Value, v => CableType.From(v));
            cable.Property(c => c.CrossSection).HasColumnName("cable_cross_section").HasColumnType("decimal(5,2)")
                .HasConversion(
                    cs => cs != null ? cs.Value : (decimal?)null,
                    v => v.HasValue ? CrossSection.From(v.Value) : null);
            cable.Property(c => c.Color).HasColumnName("cable_color").HasMaxLength(CableColor.MaxLength)
                .HasConversion(
                    c => c != null ? c.Value : null,
                    v => v != null ? CableColor.From(v) : null);
            cable.Property(c => c.ConductorCount).HasColumnName("cable_conductor_count");
        });

        builder.Property(i => i.Depth)
            .HasColumnName("depth_mm")
            .HasConversion(
                d => d != null ? d.ValueInMillimeters : (int?)null,
                value => value != null ? Depth.From(value.Value) : null);

        builder.Property(i => i.Manufacturer)
            .HasColumnName("manufacturer")
            .HasMaxLength(Manufacturer.MaxLength)
            .HasConversion(
                m => m != null ? m.Value : null,
                value => value != null ? Manufacturer.From(value) : null);

        builder.Property(i => i.ModelName)
            .HasColumnName("model_name")
            .HasMaxLength(ModelName.MaxLength)
            .HasConversion(
                m => m != null ? m.Value : null,
                value => value != null ? ModelName.From(value) : null);

        builder.Property(i => i.SerialNumber)
            .HasColumnName("serial_number")
            .HasMaxLength(SerialNumber.MaxLength)
            .HasConversion(
                s => s != null ? s.Value : null,
                value => value != null ? SerialNumber.From(value) : null);

        builder.Property(i => i.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(i => i.CompletedAt).HasColumnName("completed_at");

        builder.HasMany(i => i.Photos)
            .WithOne()
            .HasForeignKey("InstallationId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Installation.Photos))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(i => i.Measurements)
            .WithOne()
            .HasForeignKey("InstallationId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Installation.Measurements))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(i => i.DomainEvents);

        // Indexes
        builder.HasIndex(i => i.ProjectId).HasDatabaseName("ix_installations_project_id");
        builder.HasIndex(i => i.ZoneId).HasDatabaseName("ix_installations_zone_id");
        builder.HasIndex(i => i.Status).HasDatabaseName("ix_installations_status");
    }
}
