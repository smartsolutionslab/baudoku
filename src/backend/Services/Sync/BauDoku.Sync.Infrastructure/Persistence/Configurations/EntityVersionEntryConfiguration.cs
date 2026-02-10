using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Sync.Infrastructure.Persistence.Configurations;

public sealed class EntityVersionEntryConfiguration : IEntityTypeConfiguration<EntityVersionEntry>
{
    public void Configure(EntityTypeBuilder<EntityVersionEntry> builder)
    {
        builder.ToTable("entity_versions");

        builder.HasKey(e => new { e.EntityType, e.EntityId });

        builder.Property(e => e.EntityType)
            .HasColumnName("entity_type")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.EntityId)
            .HasColumnName("entity_id")
            .IsRequired();

        builder.Property(e => e.Version)
            .HasColumnName("version")
            .IsRequired();

        builder.Property(e => e.Payload)
            .HasColumnName("payload")
            .IsRequired();

        builder.Property(e => e.LastModified)
            .HasColumnName("last_modified")
            .IsRequired();

        builder.Property(e => e.LastDeviceId)
            .HasColumnName("last_device_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(e => e.LastModified)
            .HasDatabaseName("ix_entity_versions_last_modified");
    }
}
