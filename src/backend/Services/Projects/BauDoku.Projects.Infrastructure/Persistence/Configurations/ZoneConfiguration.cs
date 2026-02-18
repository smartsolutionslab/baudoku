using BauDoku.Projects.Domain.Entities;
using BauDoku.Projects.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Projects.Infrastructure.Persistence.Configurations;

public sealed class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("zones");

        builder.HasKey(z => z.Id);

        builder.Property(z => z.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => ZoneIdentifier.From(value));

        builder.Property(z => z.Name)
            .HasColumnName("name")
            .HasMaxLength(ZoneName.MaxLength)
            .HasConversion(n => n.Value, value => ZoneName.From(value));

        builder.Property(z => z.Type)
            .HasColumnName("type")
            .HasMaxLength(20)
            .HasConversion(t => t.Value, value => ZoneType.From(value));

        builder.Property(z => z.ParentZoneIdentifier)
            .HasColumnName("parent_zone_id")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => ZoneIdentifier.FromNullable(value));

        builder.Property<ProjectIdentifier>("ProjectId")
            .HasColumnName("project_id")
            .HasConversion(id => id.Value, value => ProjectIdentifier.From(value));

        // Indexes
        builder.HasIndex("ProjectId").HasDatabaseName("ix_zones_project_id");
    }
}
