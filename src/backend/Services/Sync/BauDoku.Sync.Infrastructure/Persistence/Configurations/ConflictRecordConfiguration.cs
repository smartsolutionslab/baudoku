using BauDoku.Sync.Domain.Entities;
using BauDoku.Sync.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Sync.Infrastructure.Persistence.Configurations;

public sealed class ConflictRecordConfiguration : IEntityTypeConfiguration<ConflictRecord>
{
    public void Configure(EntityTypeBuilder<ConflictRecord> builder)
    {
        builder.ToTable("sync_conflicts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => ConflictRecordIdentifier.From(value));

        builder.OwnsOne(c => c.EntityRef, entityRef =>
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

        builder.Navigation(c => c.EntityRef).IsRequired();

        builder.Property(c => c.ClientPayload)
            .HasColumnName("client_payload")
            .IsRequired()
            .HasConversion(p => p.Value, value => DeltaPayload.From(value));

        builder.Property(c => c.ServerPayload)
            .HasColumnName("server_payload")
            .IsRequired()
            .HasConversion(p => p.Value, value => DeltaPayload.From(value));

        builder.Property(c => c.ClientVersion)
            .HasColumnName("client_version")
            .IsRequired()
            .HasConversion(v => v.Value, value => SyncVersion.From(value));

        builder.Property(c => c.ServerVersion)
            .HasColumnName("server_version")
            .IsRequired()
            .HasConversion(v => v.Value, value => SyncVersion.From(value));

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(s => s.Value, value => ConflictStatus.From(value));

        builder.Property(c => c.ResolvedPayload)
            .HasColumnName("resolved_payload")
            .HasConversion(
                p => p != null ? p.Value : null,
                value => value != null ? DeltaPayload.From(value) : null);

        builder.Property(c => c.DetectedAt)
            .HasColumnName("detected_at")
            .IsRequired();

        builder.Property(c => c.ResolvedAt)
            .HasColumnName("resolved_at");
    }
}
