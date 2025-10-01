namespace CouponHub.Business.DTOs
{
    public class CouponUsageDto
    {
        public string Period { get; set; } = string.Empty; // e.g., "Jan 2024", "Week 1", etc.
        public int TotalCoupons { get; set; }
        public int RedeemedCoupons { get; set; }
        public decimal RedemptionRate { get; set; } // Percentage
        public int ActiveCoupons { get; set; }
        public int ExpiredCoupons { get; set; }
    }
}

