using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveStation.Data.Models;

namespace SolveStation.Data.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("Prescriptions");

        builder.HasKey(p => p.Id);

        // Configure PrescriptionId value conversion
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new PrescriptionId(value));

        // Configure UserId conversions
        builder.Property(p => p.PatientId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(p => p.DoctorId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(p => p.FilledByPharmacistId)
            .HasConversion(
                id => id!.Value,
                value => new UserId(value));

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Notes)
            .HasMaxLength(2000);

        builder.Property(p => p.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.FilledAt);

        builder.Property(p => p.ExpiresAt);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure relationships
        builder.HasMany(p => p.Items)
            .WithOne(pi => pi.Prescription)
            .HasForeignKey(pi => pi.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(p => p.PatientId);
        builder.HasIndex(p => p.DoctorId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.ExpiresAt);
        builder.HasIndex(p => p.FilledByPharmacistId);
        builder.HasIndex(p => p.IsActive);

        // Composite indexes
        builder.HasIndex(p => new { p.PatientId, p.Status });
        builder.HasIndex(p => new { p.DoctorId, p.CreatedAt });

        // Ignore domain events
        builder.Ignore(p => p.DomainEvents);
    }
}
