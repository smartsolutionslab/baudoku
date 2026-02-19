using BauDoku.Sync.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Sync.Infrastructure.Persistence.Configurations;

public sealed class SyncDeltaConfiguration : IEntityTypeConfiguration<SyncDelta>
{
    public void Configure(EntityTypeBuilder<SyncDelta> builder)
    {
        builder.ToTable("sync_deltas");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => SyncDeltaIdentifier.From(value));

        builder.OwnsOne(d => d.EntityRef, entityRef =>
        {
            entityRef.Property(e => e.EntityType)
                .HasColumnName("entity_type")
                .HasMaxLength(30)
                .IsRequired()
                .HasConversion(t => t.Value, value => EntityType.From(value));

            entityRef.Property(e => e.EntityId)
                .HasColumnName("entity_id")
                .IsRequired();
        });

        builder.Navigation(d => d.EntityRef).IsRequired();

        builder.Property(d => d.Operation)
            .HasColumnName("operation")
            .HasMaxLength(10)
            .IsRequired()
            .HasConversion(o => o.Value, value => DeltaOperation.From(value));

        builder.Property(d => d.BaseVersion)
            .HasColumnName("base_version")
            .IsRequired()
            .HasConversion(v => v.Value, value => SyncVersion.From(value));

        builder.Property(d => d.ServerVersion)
            .HasColumnName("server_version")
            .IsRequired()
            .HasConversion(v => v.Value, value => SyncVersion.From(value));

        builder.Property(d => d.Payload)
            .HasColumnName("payload")
            .IsRequired()
            .HasConversion(p => p.Value, value => DeltaPayload.From(value));

        builder.Property(d => d.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();
    }
}
