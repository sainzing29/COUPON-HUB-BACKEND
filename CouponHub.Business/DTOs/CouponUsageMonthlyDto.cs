namespace CouponHub.Business.DTOs
{
    public class CouponUsageMonthlyDto
    {
        public string Period { get; set; } = string.Empty; // e.g., "Jan 2024", "Feb 2024", etc.
        public int Year { get; set; }
        public int TotalCoupons { get; set; }
        public int RedeemedCoupons { get; set; }
        public decimal RedemptionRate { get; set; } // Percentage
        public int ActiveCoupons { get; set; }
        public int ExpiredCoupons { get; set; }
        public int UnassignedCoupons { get; set; }
        public int CompletedCoupons { get; set; }
    }
}

