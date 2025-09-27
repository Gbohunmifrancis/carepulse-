using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveStation.Data.Models;

namespace SolveStation.Data.Configurations;

public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
{
    public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
    {
        builder.ToTable("PrescriptionItems");

        builder.HasKey(pi => pi.Id);

        // Configure PrescriptionItemId value conversion
        builder.Property(pi => pi.Id)
            .HasConversion(
                id => id.Value,
                value => new PrescriptionItemId(value));

        // Configure foreign key conversions
        builder.Property(pi => pi.PrescriptionId)
            .HasConversion(
                id => id.Value,
                value => new PrescriptionId(value))
            .IsRequired();

        builder.Property(pi => pi.DrugId)
            .HasConversion(
                id => id.Value,
                value => new DrugId(value))
            .IsRequired();

        builder.Property(pi => pi.DrugName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pi => pi.QuantityPrescribed)
            .IsRequired();

        builder.Property(pi => pi.QuantityFilled)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pi => pi.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pi => pi.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pi => pi.Instructions)
            .HasMaxLength(1000);

        builder.Property(pi => pi.CreatedAt)
            .IsRequired();

        builder.Property(pi => pi.UpdatedAt)
            .IsRequired();

        builder.Property(pi => pi.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure relationships
        builder.HasOne(pi => pi.Prescription)
            .WithMany(p => p.Items)
            .HasForeignKey(pi => pi.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.Drug)
            .WithMany()
            .HasForeignKey(pi => pi.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(pi => pi.PrescriptionId);
        builder.HasIndex(pi => pi.DrugId);
        builder.HasIndex(pi => pi.IsActive);

        // Composite indexes
        builder.HasIndex(pi => new { pi.PrescriptionId, pi.DrugId })
            .IsUnique();

        // Ignore domain events
        builder.Ignore(pi => pi.DomainEvents);
    }
}
