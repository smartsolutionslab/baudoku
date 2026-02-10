using BauDoku.Sync.Domain.Entities;
using BauDoku.Sync.Domain.ValueObjects;
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
            .HasConversion(id => id.Value, value => new SyncDeltaId(value));

        builder.OwnsOne(d => d.EntityRef, entityRef =>
        {
            entityRef.Property(e => e.EntityType)
                .HasColumnName("entity_type")
                .HasMaxLength(30)
                .IsRequired()
                .HasConversion(t => t.Value, value => new EntityType(value));

            entityRef.Property(e => e.EntityId)
                .HasColumnName("entity_id")
                .IsRequired();
        });

        builder.Navigation(d => d.EntityRef).IsRequired();

        builder.Property(d => d.Operation)
            .HasColumnName("operation")
            .HasMaxLength(10)
            .IsRequired()
            .HasConversion(o => o.Value, value => new DeltaOperation(value));

        builder.Property(d => d.BaseVersion)
            .HasColumnName("base_version")
            .IsRequired()
            .HasConversion(v => v.Value, value => new SyncVersion(value));

        builder.Property(d => d.ServerVersion)
            .HasColumnName("server_version")
            .IsRequired()
            .HasConversion(v => v.Value, value => new SyncVersion(value));

        builder.Property(d => d.Payload)
            .HasColumnName("payload")
            .IsRequired()
            .HasConversion(p => p.Value, value => new DeltaPayload(value));

        builder.Property(d => d.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();
    }
}
