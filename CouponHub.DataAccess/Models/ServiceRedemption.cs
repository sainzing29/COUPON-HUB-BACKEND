using System.ComponentModel.DataAnnotations;

namespace CouponHub.DataAccess.Models
{
    public class ServiceRedemption
{
    public int Id { get; set; }

    public int CouponId { get; set; }
    public Coupon Coupon { get; set; } = null!;

    public int ServiceCenterId { get; set; }
    public ServiceCenter ServiceCenter { get; set; } = null!;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime RedemptionDate { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;

    // Optional invoice link (nullable)
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    }
}
