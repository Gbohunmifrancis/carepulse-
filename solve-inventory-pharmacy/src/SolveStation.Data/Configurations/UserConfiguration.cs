using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveStation.Data.Models;

namespace SolveStation.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Configure UserId value conversion
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        // Configure Email value conversion
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.Role);

        builder.HasIndex(u => u.IsActive);

        builder.HasIndex(u => u.CreatedAt);
    }
}
