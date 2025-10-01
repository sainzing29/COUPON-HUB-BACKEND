namespace CouponHub.Business.DTOs
{
    public class ServiceCenterMonthlyStatsDto
    {
        public int ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public int ServicesCompleted { get; set; }
        public int CouponsRedeemed { get; set; }
        public decimal Revenue { get; set; }
    }
}

