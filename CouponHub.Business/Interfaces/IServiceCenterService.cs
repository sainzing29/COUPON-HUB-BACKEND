using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface IServiceCenterService
    {
        Task<ServiceCenter> CreateServiceCenterAsync(ServiceCenter serviceCenter);
        Task<ServiceCenter?> GetServiceCenterByIdAsync(int id);
        Task<IEnumerable<ServiceCenter>> GetAllServiceCentersAsync();
        Task<ServiceCenter> UpdateServiceCenterAsync(ServiceCenter serviceCenter);
        Task<bool> DeleteServiceCenterAsync(int id);
    }
}


