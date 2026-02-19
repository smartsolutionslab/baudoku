using BauDoku.Sync.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Sync.Infrastructure.Persistence.Configurations;

public sealed class SyncBatchConfiguration : IEntityTypeConfiguration<SyncBatch>
{
    public void Configure(EntityTypeBuilder<SyncBatch> builder)
    {
        builder.ToTable("sync_batches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => SyncBatchIdentifier.From(value));

        builder.Property(b => b.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(DeviceIdentifier.MaxLength)
            .IsRequired()
            .HasConversion(d => d.Value, value => DeviceIdentifier.From(value));

        builder.Property(b => b.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired()
            .HasConversion(s => s.Value, value => BatchStatus.From(value));

        builder.Property(b => b.SubmittedAt)
            .HasColumnName("submitted_at")
            .IsRequired();

        builder.Property(b => b.ProcessedAt)
            .HasColumnName("processed_at");

        builder.HasMany(b => b.Deltas)
            .WithOne()
            .HasForeignKey("sync_batch_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(SyncBatch.Deltas))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(b => b.Conflicts)
            .WithOne()
            .HasForeignKey("sync_batch_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(SyncBatch.Conflicts))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(b => b.DomainEvents);
    }
}
