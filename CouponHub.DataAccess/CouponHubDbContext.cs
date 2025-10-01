using Microsoft.EntityFrameworkCore;
using CouponHub.DataAccess.Models;

namespace CouponHub.DataAccess
{
    public class CouponHubDbContext : DbContext
    {
        public CouponHubDbContext(DbContextOptions<CouponHubDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ServiceCenter> ServiceCenters { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ServiceRedemption> ServiceRedemptions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraints
        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        modelBuilder.Entity<Coupon>()
            .HasIndex(c => c.CouponCode)
            .IsUnique();

        // User relationships (Admin/SuperAdmin only)
        modelBuilder.Entity<ServiceCenter>()
            .HasMany(sc => sc.Admins)
            .WithOne(u => u.ServiceCenter)
            .HasForeignKey(u => u.ServiceCenterId);

        // Customer relationships
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Coupons)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId);

        // Coupon to ServiceCenter relationship
        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.ServiceCenter)
            .WithMany(sc => sc.Coupons)
            .HasForeignKey(c => c.ServiceCenterId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Invoices)
            .WithOne(i => i.Customer)
            .HasForeignKey(i => i.CustomerId);

        // ServiceRedemption relationships
        modelBuilder.Entity<ServiceRedemption>()
            .HasOne(sr => sr.Coupon)
            .WithMany(c => c.Redemptions)
            .HasForeignKey(sr => sr.CouponId);

        modelBuilder.Entity<ServiceRedemption>()
            .HasOne(sr => sr.ServiceCenter)
            .WithMany(sc => sc.Redemptions)
            .HasForeignKey(sr => sr.ServiceCenterId);

        modelBuilder.Entity<ServiceRedemption>()
            .HasOne(sr => sr.Customer)
            .WithMany()
            .HasForeignKey(sr => sr.CustomerId);

        modelBuilder.Entity<ServiceRedemption>()
            .HasOne(sr => sr.Invoice)
            .WithMany(i => i.ServiceRedemptions)
            .HasForeignKey(sr => sr.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Invoice relationships
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Coupon)
            .WithMany()
            .HasForeignKey(i => i.CouponId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.ServiceCenter)
            .WithMany(sc => sc.Invoices)
            .HasForeignKey(i => i.ServiceCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CouponStatus enum to be stored as integer (default behavior)
        // No additional configuration needed - EF will store enum as integer by default

        // PasswordResetToken relationships
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(prt => prt.User)
            .WithMany()
            .HasForeignKey(prt => prt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint on token
        modelBuilder.Entity<PasswordResetToken>()
            .HasIndex(prt => prt.Token)
            .IsUnique();
    }

    }
}
