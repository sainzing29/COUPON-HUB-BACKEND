using CouponHub.Business.Interfaces;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponHub.Business.Services
{
    public class ServiceRedemptionService : IServiceRedemptionService
    {
        private readonly CouponHubDbContext _context;

        public ServiceRedemptionService(CouponHubDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRedemption> CreateServiceRedemptionAsync(ServiceRedemption serviceRedemption)
        {
            _context.ServiceRedemptions.Add(serviceRedemption);
            await _context.SaveChangesAsync();
            return serviceRedemption;
        }

        public async Task<ServiceRedemption?> GetServiceRedemptionByIdAsync(int id)
        {
            return await _context.ServiceRedemptions
                .Include(sr => sr.Coupon)
                .Include(sr => sr.ServiceCenter)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Invoice)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<IEnumerable<ServiceRedemption>> GetAllServiceRedemptionsAsync()
        {
            return await _context.ServiceRedemptions
                .Include(sr => sr.Coupon)
                .Include(sr => sr.ServiceCenter)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Invoice)
                .OrderByDescending(sr => sr.RedemptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByCustomerIdAsync(int customerId)
        {
            return await _context.ServiceRedemptions
                .Include(sr => sr.Coupon)
                .Include(sr => sr.ServiceCenter)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Invoice)
                .Where(sr => sr.CustomerId == customerId)
                .OrderByDescending(sr => sr.RedemptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByServiceCenterIdAsync(int serviceCenterId)
        {
            return await _context.ServiceRedemptions
                .Include(sr => sr.Coupon)
                .Include(sr => sr.ServiceCenter)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Invoice)
                .Where(sr => sr.ServiceCenterId == serviceCenterId)
                .OrderByDescending(sr => sr.RedemptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByCouponIdAsync(int couponId)
        {
            return await _context.ServiceRedemptions
                .Include(sr => sr.Coupon)
                .Include(sr => sr.ServiceCenter)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Invoice)
                .Where(sr => sr.CouponId == couponId)
                .OrderByDescending(sr => sr.RedemptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRedemption>> GetServiceRedemptionsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.ServiceRedemptions
                .Include(sr => sr.Coupon)
                .Include(sr => sr.ServiceCenter)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Invoice)
                .Where(sr => sr.InvoiceId == invoiceId)
                .OrderByDescending(sr => sr.RedemptionDate)
                .ToListAsync();
        }

        public async Task<ServiceRedemption> UpdateServiceRedemptionAsync(ServiceRedemption serviceRedemption)
        {
            _context.ServiceRedemptions.Update(serviceRedemption);
            await _context.SaveChangesAsync();
            return serviceRedemption;
        }

        public async Task<bool> DeleteServiceRedemptionAsync(int id)
        {
            var serviceRedemption = await _context.ServiceRedemptions.FindAsync(id);
            if (serviceRedemption == null)
                return false;

            _context.ServiceRedemptions.Remove(serviceRedemption);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


