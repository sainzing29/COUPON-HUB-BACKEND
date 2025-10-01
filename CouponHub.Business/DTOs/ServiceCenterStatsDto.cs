namespace CouponHub.Business.DTOs
{
    public class ServiceCenterStatsDto
    {
        public int ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; } = string.Empty;
        public int ServicesCompleted { get; set; }
        public int CouponsRedeemed { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MonthlyAggregatedData
    {
        public string Month { get; set; } = string.Empty;
        public int ServicesCompleted { get; set; }
        public int CouponsRedeemed { get; set; }
        public decimal Revenue { get; set; }
    }
}

