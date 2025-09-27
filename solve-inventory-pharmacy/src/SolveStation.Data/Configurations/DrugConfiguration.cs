using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveStation.Data.Models;

namespace SolveStation.Data.Configurations;

public class DrugConfiguration : IEntityTypeConfiguration<Drug>
{
    public void Configure(EntityTypeBuilder<Drug> builder)
    {
        builder.ToTable("Drugs");

        builder.HasKey(d => d.Id);

        // Configure DrugId value conversion
        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => new DrugId(value));

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.StockQuantity)
            .IsRequired();

        builder.Property(d => d.MinimumStockLevel)
            .IsRequired();

        builder.Property(d => d.ExpiryDate)
            .IsRequired();

        builder.Property(d => d.Supplier)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.BatchNumber)
            .HasMaxLength(100);

        builder.Property(d => d.DrugCode)
            .HasMaxLength(50);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes for performance
        builder.HasIndex(d => d.Name)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.HasIndex(d => d.IsActive);

        builder.HasIndex(d => d.ExpiryDate);

        builder.HasIndex(d => d.StockQuantity);

        builder.HasIndex(d => d.DrugCode)
            .IsUnique()
            .HasFilter("[DrugCode] IS NOT NULL AND [IsActive] = 1");

        builder.HasIndex(d => d.Supplier);

        // Ignore domain events (they're handled separately)
        builder.Ignore(d => d.DomainEvents);
    }
}
