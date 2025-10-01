using System.ComponentModel.DataAnnotations;

namespace CouponHub.DataAccess.Models
{
    public enum CouponStatus
    {
        Unassigned,
        Active,
        Completed,
        Expired
    }

    public class Coupon
{
    public int Id { get; set; }
    public string CouponCode { get; set; } = string.Empty;

    // Customer (not User anymore)
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int? ServiceCenterId { get; set; }
    public ServiceCenter? ServiceCenter { get; set; }

    public int TotalServices { get; set; } = 5;
    public int UsedServices { get; set; } = 0;

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public CouponStatus Status { get; set; } = CouponStatus.Unassigned;

    public ICollection<ServiceRedemption> Redemptions { get; set; } = new List<ServiceRedemption>();
    }
}
