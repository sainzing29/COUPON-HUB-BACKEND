using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using CouponHub.Business.DTOs;
using CouponHub.Business.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace CouponHub.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly CouponHubDbContext _context;

        public DashboardService(CouponHubDbContext context)
        {
            _context = context;
        }

public async Task<DashboardStatsDto> GetDashboardStatsAsync(int? serviceCenterId = null)
{
    var dto = new DashboardStatsDto();

    // Base queries
    var couponsQuery = _context.Coupons.AsQueryable();
    var invoicesQuery = _context.Invoices.AsQueryable();
    var redemptionsQuery = _context.ServiceRedemptions.AsQueryable();

    // If filtering by service center
    if (serviceCenterId.HasValue)
    {
        couponsQuery = couponsQuery.Where(c => c.ServiceCenterId == serviceCenterId);
        invoicesQuery = invoicesQuery.Where(i => i.ServiceCenterId == serviceCenterId);
        redemptionsQuery = redemptionsQuery.Where(r => r.ServiceCenterId == serviceCenterId);
    }

    var lastMonth = DateTime.UtcNow.AddMonths(-1);
    var thisMonth = DateTime.UtcNow;

    dto.TotalCouponsSold = await couponsQuery
        .CountAsync(c => c.Status != CouponStatus.Unassigned);

    dto.CouponsSoldLastMonth = await invoicesQuery
        .CountAsync(i => i.CreatedAt.Month == lastMonth.Month && i.CreatedAt.Year == lastMonth.Year);

    dto.CouponsSoldThisMonth = await invoicesQuery
        .CountAsync(i => i.CreatedAt.Month == thisMonth.Month && i.CreatedAt.Year == thisMonth.Year);

    dto.ActiveCoupons = await couponsQuery.CountAsync(c => c.Status == CouponStatus.Active);

    dto.ServicesCompleted = await redemptionsQuery.CountAsync();

    dto.ServicesCompletedThisMonth = await redemptionsQuery
        .CountAsync(r => r.RedemptionDate.Month == thisMonth.Month && r.RedemptionDate.Year == thisMonth.Year);

    dto.TotalRevenue = await invoicesQuery
        .Where(i => i.PaymentStatus == "Paid")
        .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

    dto.RevenueThisMonth = await invoicesQuery
        .Where(i => i.PaymentStatus == "Paid" &&
                    i.CreatedAt.Month == thisMonth.Month &&
                    i.CreatedAt.Year == thisMonth.Year)
        .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

    dto.RevenueLastMonth = await invoicesQuery
        .Where(i => i.PaymentStatus == "Paid" &&
                    i.CreatedAt.Month == lastMonth.Month &&
                    i.CreatedAt.Year == lastMonth.Year)
        .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

    return dto;
}

        public async Task<IEnumerable<SalesTrendDto>> GetSalesTrendsAsync(int? serviceCenterId = null, int months = 12)
        {
            var invoicesQuery = _context.Invoices.AsQueryable();
            
            if (serviceCenterId.HasValue)
            {
                invoicesQuery = invoicesQuery.Where(i => i.ServiceCenterId == serviceCenterId);
            }

            var trends = new List<SalesTrendDto>();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var monthName = targetDate.ToString("MMM yyyy");

                var monthInvoices = await invoicesQuery
                    .Where(i => i.CreatedAt.Month == targetDate.Month && 
                               i.CreatedAt.Year == targetDate.Year)
                    .ToListAsync();

                var revenue = monthInvoices
                    .Where(i => i.PaymentStatus == "Paid")
                    .Sum(i => i.TotalAmount);

                var couponsSold = monthInvoices.Count;

                trends.Add(new SalesTrendDto
                {
                    Month = monthName,
                    Year = targetDate.Year,
                    Revenue = revenue,
                    CouponsSold = couponsSold
                });
            }

            return trends;
        }

        public async Task<IEnumerable<ServiceCenterStatsDto>> GetServiceCenterStatsAsync(int? serviceCenterId = null, int months = 12)
        {
            var serviceCentersQuery = _context.ServiceCenters.AsQueryable();
            
            if (serviceCenterId.HasValue)
            {
                serviceCentersQuery = serviceCentersQuery.Where(sc => sc.Id == serviceCenterId);
            }

            var serviceCenters = await serviceCentersQuery
                .Include(sc => sc.Redemptions)
                .Include(sc => sc.Invoices)
                .ToListAsync();

            var cutoffDate = DateTime.UtcNow.AddMonths(-months);

            var stats = serviceCenters.Select(sc => new ServiceCenterStatsDto
            {
                ServiceCenterId = sc.Id,
                ServiceCenterName = sc.Name,
                ServicesCompleted = sc.Redemptions?.Count(r => r.RedemptionDate >= cutoffDate) ?? 0,
                CouponsRedeemed = sc.Redemptions?.Count(r => r.RedemptionDate >= cutoffDate) ?? 0,
                Revenue = sc.Invoices?.Where(i => i.PaymentStatus == "Paid" && i.CreatedAt >= cutoffDate).Sum(i => i.TotalAmount) ?? 0
            }).OrderByDescending(s => s.ServicesCompleted);

            return stats;
        }

        public async Task<IEnumerable<ServiceCenterMonthlyStatsDto>> GetServiceCenterMonthlyStatsAsync(int? serviceCenterId = null, int months = 12)
        {
            var serviceCentersQuery = _context.ServiceCenters.AsQueryable();
            
            if (serviceCenterId.HasValue)
            {
                serviceCentersQuery = serviceCentersQuery.Where(sc => sc.Id == serviceCenterId);
            }

            var serviceCenters = await serviceCentersQuery
                .Include(sc => sc.Redemptions)
                .Include(sc => sc.Invoices)
                .ToListAsync();

            var monthlyStats = new List<ServiceCenterMonthlyStatsDto>();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var monthName = targetDate.ToString("MMM yyyy");

                foreach (var sc in serviceCenters)
                {
                    var monthRedemptions = sc.Redemptions?.Where(r => r.RedemptionDate.Month == targetDate.Month && 
                                                                     r.RedemptionDate.Year == targetDate.Year) ?? new List<ServiceRedemption>();
                    
                    var monthInvoices = sc.Invoices?.Where(i => i.CreatedAt.Month == targetDate.Month && 
                                                               i.CreatedAt.Year == targetDate.Year) ?? new List<Invoice>();

                    var revenue = monthInvoices
                        .Where(i => i.PaymentStatus == "Paid")
                        .Sum(i => i.TotalAmount);

                    monthlyStats.Add(new ServiceCenterMonthlyStatsDto
                    {
                        ServiceCenterId = sc.Id,
                        ServiceCenterName = sc.Name,
                        Month = monthName,
                        Year = targetDate.Year,
                        ServicesCompleted = monthRedemptions.Count(),
                        CouponsRedeemed = monthRedemptions.Count(),
                        Revenue = revenue
                    });
                }
            }

            return monthlyStats.OrderBy(s => s.ServiceCenterName).ThenBy(s => s.Year).ThenBy(s => s.Month);
        }

        public async Task<IEnumerable<CouponUsageDto>> GetCouponUsageAsync(int? serviceCenterId = null, int months = 6)
        {
            var couponsQuery = _context.Coupons.AsQueryable();
            var redemptionsQuery = _context.ServiceRedemptions.AsQueryable();

            if (serviceCenterId.HasValue)
            {
                couponsQuery = couponsQuery.Where(c => c.ServiceCenterId == serviceCenterId);
                redemptionsQuery = redemptionsQuery.Where(r => r.ServiceCenterId == serviceCenterId);
            }

            var usage = new List<CouponUsageDto>();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var period = targetDate.ToString("MMM yyyy");

                var monthCoupons = await couponsQuery
                    .Where(c => c.PurchaseDate.Month == targetDate.Month && 
                               c.PurchaseDate.Year == targetDate.Year)
                    .ToListAsync();

                var monthRedemptions = await redemptionsQuery
                    .Where(r => r.RedemptionDate.Month == targetDate.Month && 
                               r.RedemptionDate.Year == targetDate.Year)
                    .ToListAsync();

                var totalCoupons = monthCoupons.Count;
                var redeemedCoupons = monthRedemptions.Count;
                var redemptionRate = totalCoupons > 0 ? (decimal)redeemedCoupons / totalCoupons * 100 : 0;

                var activeCoupons = monthCoupons.Count(c => c.Status == CouponStatus.Active);
                var expiredCoupons = monthCoupons.Count(c => c.Status == CouponStatus.Expired);

                usage.Add(new CouponUsageDto
                {
                    Period = period,
                    TotalCoupons = totalCoupons,
                    RedeemedCoupons = redeemedCoupons,
                    RedemptionRate = Math.Round(redemptionRate, 2),
                    ActiveCoupons = activeCoupons,
                    ExpiredCoupons = expiredCoupons
                });
            }

            return usage;
        }

        public async Task<IEnumerable<CouponUsageMonthlyDto>> GetCouponUsageMonthlyAsync(int? serviceCenterId = null, int months = 12)
        {
            var couponsQuery = _context.Coupons.AsQueryable();
            var redemptionsQuery = _context.ServiceRedemptions.AsQueryable();

            if (serviceCenterId.HasValue)
            {
                couponsQuery = couponsQuery.Where(c => c.ServiceCenterId == serviceCenterId);
                redemptionsQuery = redemptionsQuery.Where(r => r.ServiceCenterId == serviceCenterId);
            }

            var usage = new List<CouponUsageMonthlyDto>();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var period = targetDate.ToString("MMM yyyy");

                var monthCoupons = await couponsQuery
                    .Where(c => c.PurchaseDate.Month == targetDate.Month && 
                               c.PurchaseDate.Year == targetDate.Year)
                    .ToListAsync();

                var monthRedemptions = await redemptionsQuery
                    .Where(r => r.RedemptionDate.Month == targetDate.Month && 
                               r.RedemptionDate.Year == targetDate.Year)
                    .ToListAsync();

                var totalCoupons = monthCoupons.Count;
                var redeemedCoupons = monthRedemptions.Count;
                var redemptionRate = totalCoupons > 0 ? (decimal)redeemedCoupons / totalCoupons * 100 : 0;

                var activeCoupons = monthCoupons.Count(c => c.Status == CouponStatus.Active);
                var expiredCoupons = monthCoupons.Count(c => c.Status == CouponStatus.Expired);
                var unassignedCoupons = monthCoupons.Count(c => c.Status == CouponStatus.Unassigned);
                var completedCoupons = monthCoupons.Count(c => c.Status == CouponStatus.Completed);

                usage.Add(new CouponUsageMonthlyDto
                {
                    Period = period,
                    Year = targetDate.Year,
                    TotalCoupons = totalCoupons,
                    RedeemedCoupons = redeemedCoupons,
                    RedemptionRate = Math.Round(redemptionRate, 2),
                    ActiveCoupons = activeCoupons,
                    ExpiredCoupons = expiredCoupons,
                    UnassignedCoupons = unassignedCoupons,
                    CompletedCoupons = completedCoupons
                });
            }

            return usage.OrderBy(u => u.Year).ThenBy(u => u.Period);
        }

        public async Task<IEnumerable<MonthlyAggregatedData>> GetMonthlyAggregatedServiceCenterDataAsync(int months = 12)
        {
            var redemptionsQuery = _context.ServiceRedemptions.AsQueryable();
            var invoicesQuery = _context.Invoices.AsQueryable();

            var monthlyData = new List<MonthlyAggregatedData>();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var monthName = targetDate.ToString("MMM yyyy");

                var monthRedemptions = await redemptionsQuery
                    .Where(r => r.RedemptionDate.Month == targetDate.Month && 
                               r.RedemptionDate.Year == targetDate.Year)
                    .ToListAsync();

                var monthInvoices = await invoicesQuery
                    .Where(i => i.CreatedAt.Month == targetDate.Month && 
                               i.CreatedAt.Year == targetDate.Year)
                    .ToListAsync();

                var revenue = monthInvoices
                    .Where(i => i.PaymentStatus == "Paid")
                    .Sum(i => i.TotalAmount);

                monthlyData.Add(new MonthlyAggregatedData
                {
                    Month = monthName,
                    ServicesCompleted = monthRedemptions.Count,
                    CouponsRedeemed = monthRedemptions.Count,
                    Revenue = revenue
                });
            }

            return monthlyData;
        }

        public async Task<DashboardWidgetsDto> GetDashboardWidgetsAsync(int? serviceCenterId = null, int months = 12)
        {
            var widgets = new DashboardWidgetsDto();

            // Get Sales Trends Data
            var salesTrends = await GetSalesTrendsAsync(serviceCenterId, months);
            widgets.SalesTrends.Data = salesTrends.Select(st => new SalesTrendData
            {
                Month = st.Month,
                Year = st.Year,
                Revenue = st.Revenue,
                CouponsSold = st.CouponsSold
            }).ToList();

            // Get Service Center Distribution Data (monthly trends)
            if (serviceCenterId.HasValue)
            {
                // Admin user - show monthly data for their specific service center
                var serviceCenterMonthlyStats = await GetServiceCenterMonthlyStatsAsync(serviceCenterId, months);
                widgets.ServiceCenterDistribution.Data = serviceCenterMonthlyStats.Select(sc => new ServiceCenterData
                {
                    ServiceCenterId = sc.ServiceCenterId,
                    ServiceCenterName = sc.Month, // Use month as the label
                    ServicesCompleted = sc.ServicesCompleted,
                    CouponsRedeemed = sc.CouponsRedeemed,
                    Revenue = sc.Revenue
                }).ToList();
            }
            else
            {
                // SuperAdmin user - show aggregated monthly data across all centers
                var monthlyAggregatedData = await GetMonthlyAggregatedServiceCenterDataAsync(months);
                widgets.ServiceCenterDistribution.Data = monthlyAggregatedData.Select(data => new ServiceCenterData
                {
                    ServiceCenterId = 0, // No specific center ID for aggregated data
                    ServiceCenterName = data.Month, // Use month as the label
                    ServicesCompleted = data.ServicesCompleted,
                    CouponsRedeemed = data.CouponsRedeemed,
                    Revenue = data.Revenue
                }).ToList();
            }

            // Get Coupon Usage Data
            var couponUsage = await GetCouponUsageMonthlyAsync(serviceCenterId, months);
            widgets.CouponUsage.Data = couponUsage.Select(cu => new CouponUsageData
            {
                Period = cu.Period,
                Year = cu.Year,
                TotalCoupons = cu.TotalCoupons,
                RedeemedCoupons = cu.RedeemedCoupons,
                RedemptionRate = cu.RedemptionRate,
                ActiveCoupons = cu.ActiveCoupons,
                ExpiredCoupons = cu.ExpiredCoupons,
                UnassignedCoupons = cu.UnassignedCoupons,
                CompletedCoupons = cu.CompletedCoupons
            }).ToList();

            return widgets;
        }
    }
}

