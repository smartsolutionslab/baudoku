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
            .HasConversion(id => id.Value, value => new MeasurementId(value));

        builder.Property(m => m.Type)
            .HasColumnName("type")
            .HasMaxLength(30)
            .IsRequired()
            .HasConversion(t => t.Value, value => new MeasurementType(value));

        builder.OwnsOne(m => m.Value, val =>
        {
            val.Property(v => v.Value).HasColumnName("measurement_value").IsRequired();
            val.Property(v => v.Unit).HasColumnName("measurement_unit").HasMaxLength(MeasurementValue.MaxUnitLength).IsRequired();
        });

        builder.Navigation(m => m.Value).IsRequired();

        builder.Property(m => m.MeasuredAt)
            .HasColumnName("measured_at")
            .IsRequired();

        builder.Property(m => m.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500);

        builder.Property<InstallationId>("InstallationId")
            .HasColumnName("installation_id")
            .HasConversion(id => id.Value, value => new InstallationId(value));
    }
}
