using CouponHub.Business.DTOs;

namespace CouponHub.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(int? serviceCenterId = null);
        Task<IEnumerable<SalesTrendDto>> GetSalesTrendsAsync(int? serviceCenterId = null, int months = 12);
        Task<IEnumerable<ServiceCenterStatsDto>> GetServiceCenterStatsAsync(int? serviceCenterId = null, int months = 12);
        Task<IEnumerable<ServiceCenterMonthlyStatsDto>> GetServiceCenterMonthlyStatsAsync(int? serviceCenterId = null, int months = 12);
        Task<IEnumerable<CouponUsageDto>> GetCouponUsageAsync(int? serviceCenterId = null, int months = 6);
        Task<IEnumerable<CouponUsageMonthlyDto>> GetCouponUsageMonthlyAsync(int? serviceCenterId = null, int months = 12);
        Task<IEnumerable<MonthlyAggregatedData>> GetMonthlyAggregatedServiceCenterDataAsync(int months = 12);
        Task<DashboardWidgetsDto> GetDashboardWidgetsAsync(int? serviceCenterId = null, int months = 12);
    }
}
