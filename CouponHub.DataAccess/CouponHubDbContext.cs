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
    }

    }
}
