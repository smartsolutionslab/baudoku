using BauDoku.Documentation.Domain.Entities;
using BauDoku.Documentation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Documentation.Infrastructure.Persistence.Configurations;

public sealed class MeasurementConfiguration : IEntityTypeConfiguration<Measurement>
{
    public void Configure(EntityTypeBuilder<Measurement> builder)
    {
        builder.ToTable("measurements");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => MeasurementIdentifier.From(value));

        builder.Property(m => m.Type)
            .HasColumnName("type")
            .HasMaxLength(30)
            .IsRequired()
            .HasConversion(t => t.Value, value => MeasurementType.From(value));

        builder.OwnsOne(m => m.Value, val =>
        {
            val.Property(v => v.Value).HasColumnName("measurement_value").IsRequired();
            val.Property(v => v.Unit).HasColumnName("measurement_unit").HasMaxLength(MeasurementUnit.MaxLength).IsRequired()
                .HasConversion(u => u.Value, v => MeasurementUnit.From(v));
            val.Property(v => v.MinThreshold).HasColumnName("min_threshold");
            val.Property(v => v.MaxThreshold).HasColumnName("max_threshold");
        });

        builder.Navigation(m => m.Value).IsRequired();

        builder.Property(m => m.Result)
            .HasColumnName("result")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(r => r.Value, value => MeasurementResult.From(value));

        builder.Property(m => m.MeasuredAt)
            .HasColumnName("measured_at")
            .IsRequired();

        builder.Property(m => m.Notes)
            .HasColumnName("notes")
            .HasMaxLength(Notes.MaxLength)
            .HasConversion(
                n => n != null ? n.Value : null,
                v => v != null ? Notes.From(v) : null);

        builder.Property<InstallationIdentifier>("InstallationId")
            .HasColumnName("installation_id")
            .HasConversion(id => id.Value, value => InstallationIdentifier.From(value));
    }
}
