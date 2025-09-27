using Microsoft.EntityFrameworkCore;
using SolveStation.Data.Models;
using SolveStation.Common.Models;

namespace SolveStation.Data.Context;

public class PharmacyDbContext : DbContext
{
    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Drug> Drugs { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore DomainEvent as it's not meant to be persisted
        modelBuilder.Ignore<DomainEvent>();

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.UserId).IsRequired();
            entity.Property(rt => rt.ExpiryDate).IsRequired();
            entity.Property(rt => rt.CreatedDate).IsRequired();
            entity.Property(rt => rt.IsRevoked).IsRequired();

            // Index for faster lookups
            entity.HasIndex(rt => rt.Token);
            entity.HasIndex(rt => rt.UserId);
            entity.HasIndex(rt => new { rt.Token, rt.UserId, rt.IsRevoked });
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps before saving
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity<object> && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity<object> entity)
            {
                if (entry.State == EntityState.Added)
                {
                    entity.GetType().GetProperty("CreatedAt")?.SetValue(entity, DateTime.UtcNow);
                }
                entity.GetType().GetProperty("UpdatedAt")?.SetValue(entity, DateTime.UtcNow);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
