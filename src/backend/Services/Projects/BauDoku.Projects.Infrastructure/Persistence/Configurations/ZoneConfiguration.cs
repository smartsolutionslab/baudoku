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
            .HasConversion(id => id.Value, value => new ZoneId(value));

        builder.Property(z => z.Name)
            .HasColumnName("name")
            .HasMaxLength(ZoneName.MaxLength)
            .HasConversion(n => n.Value, value => new ZoneName(value));

        builder.Property(z => z.Type)
            .HasColumnName("type")
            .HasMaxLength(20)
            .HasConversion(t => t.Value, value => new ZoneType(value));

        builder.Property(z => z.ParentZoneId)
            .HasColumnName("parent_zone_id")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? new ZoneId(value.Value) : null);

        builder.Property<ProjectId>("ProjectId")
            .HasColumnName("project_id")
            .HasConversion(id => id.Value, value => new ProjectId(value));
    }
}
