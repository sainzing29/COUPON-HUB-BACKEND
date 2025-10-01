namespace CouponHub.Business.DTOs
{
    public class DashboardWidgetsDto
    {
        public SalesTrendsWidget SalesTrends { get; set; } = new();
        public ServiceCenterDistributionWidget ServiceCenterDistribution { get; set; } = new();
        public CouponUsageWidget CouponUsage { get; set; } = new();
    }

    public class SalesTrendsWidget
    {
        public string Title { get; set; } = "Sales Trends";
        public string Description { get; set; } = "Monthly sales performance over time";
        public string Footer { get; set; } = "Last 12 months";
        public List<SalesTrendData> Data { get; set; } = new();
    }

    public class SalesTrendData
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int CouponsSold { get; set; }
    }

    public class ServiceCenterDistributionWidget
    {
        public string Title { get; set; } = "Services by Service Center";
        public string Description { get; set; } = "Distribution of services across centers";
        public string Footer { get; set; } = "By location";
        public List<ServiceCenterData> Data { get; set; } = new();
    }

    public class ServiceCenterData
    {
        public int ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; } = string.Empty;
        public int ServicesCompleted { get; set; }
        public int CouponsRedeemed { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CouponUsageWidget
    {
        public string Title { get; set; } = "Coupon Usage";
        public string Description { get; set; } = "Usage patterns and redemption rates";
        public string Footer { get; set; } = "Redemption rate";
        public List<CouponUsageData> Data { get; set; } = new();
    }

    public class CouponUsageData
    {
        public string Period { get; set; } = string.Empty;
        public int Year { get; set; }
        public int TotalCoupons { get; set; }
        public int RedeemedCoupons { get; set; }
        public decimal RedemptionRate { get; set; }
        public int ActiveCoupons { get; set; }
        public int ExpiredCoupons { get; set; }
        public int UnassignedCoupons { get; set; }
        public int CompletedCoupons { get; set; }
    }
}

