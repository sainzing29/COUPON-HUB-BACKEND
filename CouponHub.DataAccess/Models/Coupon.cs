using System.ComponentModel.DataAnnotations;

namespace CouponHub.DataAccess.Models
{
    public class Coupon
{
    public int Id { get; set; }
    public string CouponCode { get; set; } = string.Empty;

    // Customer (not User anymore)
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int TotalServices { get; set; } = 5;
    public int UsedServices { get; set; } = 0;

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = "Active";

    public ICollection<ServiceRedemption> Redemptions { get; set; } = new List<ServiceRedemption>();
    }
}
