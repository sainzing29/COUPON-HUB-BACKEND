namespace CouponHub.Business.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalCouponsSold { get; set; }
        public int CouponsSoldLastMonth { get; set; }
        public int CouponsSoldThisMonth { get; set; }
        public int ActiveCoupons { get; set; }
        public int ServicesCompleted { get; set; }
        public int ServicesCompletedThisMonth { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueLastMonth { get; set; }
    }
}