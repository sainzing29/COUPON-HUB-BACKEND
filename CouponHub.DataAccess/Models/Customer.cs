using System.ComponentModel.DataAnnotations;

namespace CouponHub.DataAccess.Models
{
    public class Customer
{
    [Key]
    public int Id { get; set; }

    // Basic Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;

    // Login
    public string? OtpCode { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public string? GoogleId { get; set; }

    // Relations
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    // System fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    }
}
