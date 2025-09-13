using CouponHub.Business.Interfaces;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponHub.Business.Services
{
    public class ServiceCenterService : IServiceCenterService
    {
        private readonly CouponHubDbContext _context;

        public ServiceCenterService(CouponHubDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceCenter> CreateServiceCenterAsync(ServiceCenter serviceCenter)
        {
            _context.ServiceCenters.Add(serviceCenter);
            await _context.SaveChangesAsync();
            return serviceCenter;
        }

        public async Task<ServiceCenter?> GetServiceCenterByIdAsync(int id)
        {
            return await _context.ServiceCenters
                .FirstOrDefaultAsync(sc => sc.Id == id);
        }

        public async Task<IEnumerable<ServiceCenter>> GetAllServiceCentersAsync()
        {
            return await _context.ServiceCenters
                .OrderBy(sc => sc.Name)
                .ToListAsync();
        }

        public async Task<ServiceCenter> UpdateServiceCenterAsync(ServiceCenter serviceCenter)
        {
            _context.ServiceCenters.Update(serviceCenter);
            await _context.SaveChangesAsync();
            return serviceCenter;
        }

        public async Task<bool> DeleteServiceCenterAsync(int id)
        {
            var serviceCenter = await _context.ServiceCenters.FindAsync(id);
            if (serviceCenter == null)
                return false;

            _context.ServiceCenters.Remove(serviceCenter);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


