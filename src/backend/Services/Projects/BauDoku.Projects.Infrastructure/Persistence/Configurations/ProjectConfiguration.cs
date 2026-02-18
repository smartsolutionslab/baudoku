using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BauDoku.Projects.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => ProjectIdentifier.From(value));

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(ProjectName.MaxLength)
            .HasConversion(n => n.Value, value => ProjectName.From(value));

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(s => s.Value, value => ProjectStatus.From(value));

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at");

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("address_street")
                .HasMaxLength(Street.MaxLength)
                .HasConversion(s => s.Value, value => Street.From(value));

            address.Property(a => a.City)
                .HasColumnName("address_city")
                .HasMaxLength(City.MaxLength)
                .HasConversion(c => c.Value, value => City.From(value));

            address.Property(a => a.ZipCode)
                .HasColumnName("address_zip_code")
                .HasMaxLength(ZipCode.MaxLength)
                .HasConversion(z => z.Value, value => ZipCode.From(value));
        });

        builder.OwnsOne(p => p.Client, client =>
        {
            client.Property(c => c.Name)
                .HasColumnName("client_name")
                .HasMaxLength(ClientInfo.MaxNameLength);

            client.Property(c => c.Email)
                .HasColumnName("client_email")
                .HasMaxLength(ClientInfo.MaxEmailLength);

            client.Property(c => c.Phone)
                .HasColumnName("client_phone")
                .HasMaxLength(ClientInfo.MaxPhoneLength);
        });

        builder.HasMany(p => p.Zones)
            .WithOne()
            .HasForeignKey("ProjectId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Zones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.DomainEvents);

        // Indexes
        builder.HasIndex(p => p.Name).HasDatabaseName("ix_projects_name");
        builder.HasIndex(p => p.Status).HasDatabaseName("ix_projects_status");
    }
}
