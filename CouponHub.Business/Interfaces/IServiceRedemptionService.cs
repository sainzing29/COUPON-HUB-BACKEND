using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface IServiceRedemptionService
    {
        Task<ServiceRedemption> CreateServiceRedemptionAsync(ServiceRedemption serviceRedemption);
        Task<ServiceRedemption?> GetServiceRedemptionByIdAsync(int id);
        Task<IEnumerable<ServiceRedemption>> GetAllServiceRedemptionsAsync();
        Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByCustomerIdAsync(int customerId);
        Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByServiceCenterIdAsync(int serviceCenterId);
        Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByCouponIdAsync(int couponId);
        Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByInvoiceIdAsync(int invoiceId);
        Task<ServiceRedemption> UpdateServiceRedemptionAsync(ServiceRedemption serviceRedemption);
        Task<bool> DeleteServiceRedemptionAsync(int id);
    }
}


