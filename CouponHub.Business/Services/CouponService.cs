using CouponHub.Business.Interfaces;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponHub.Business.Services
{
    public class CouponService : ICouponService
    {
        private readonly CouponHubDbContext _context;

        public CouponService(CouponHubDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon> CreateCouponAsync(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<Coupon?> GetCouponByIdAsync(int id)
        {
            return await _context.Coupons
                .Include(c => c.Customer)
                .Include(c => c.Redemptions)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Coupon?> GetCouponByCodeAsync(string couponCode)
        {
            return await _context.Coupons
                .Include(c => c.Customer)
                .Include(c => c.Redemptions)
                .FirstOrDefaultAsync(c => c.CouponCode == couponCode);
        }

        public async Task<IEnumerable<Coupon>> GetAllCouponsAsync()
        {
            return await _context.Coupons
                .Include(c => c.Customer)
                .Include(c => c.Redemptions)
                .OrderByDescending(c => c.PurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetCouponsByCustomerIdAsync(int customerId)
        {
            return await _context.Coupons
                .Include(c => c.Customer)
                .Include(c => c.Redemptions)
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.PurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            return await _context.Coupons
                .Include(c => c.Customer)
                .Include(c => c.Redemptions)
                .Where(c => c.Status == CouponStatus.Active)
                .OrderByDescending(c => c.PurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetUnassignedCouponsAsync()
        {
            return await _context.Coupons
                .Include(c => c.Customer)
                .Include(c => c.Redemptions)
                .Where(c => c.Status == CouponStatus.Unassigned)
                .OrderByDescending(c => c.PurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetExpiredCouponsAsync()
        {
            return await _context.Coupons
                .Include(c => c.Customer)
                .Include(c => c.Redemptions)
                .Where(c => c.Status == CouponStatus.Expired)
                .OrderByDescending(c => c.PurchaseDate)
                .ToListAsync();
        }

    

        public async Task<Coupon> UpdateCouponAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
                return false;

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RedeemCouponAsync(int couponId, int serviceCenterId, int customerId, string notes = "")
        {
            var coupon = await _context.Coupons.FindAsync(couponId);
            if (coupon == null || coupon.Status !=CouponStatus.Active|| coupon.ExpiryDate <= DateTime.UtcNow)
                return false;

            if (coupon.UsedServices >= coupon.TotalServices)
                return false;

            // Create a new service redemption
            var redemption = new ServiceRedemption
            {
                CouponId = couponId,
                ServiceCenterId = serviceCenterId,
                CustomerId = customerId,
                RedemptionDate = DateTime.UtcNow,
                Notes = notes
            };

            _context.ServiceRedemptions.Add(redemption);

            // Update coupon usage
            coupon.UsedServices++;

            // Check if all services are used
            if (coupon.UsedServices >= coupon.TotalServices)
            {
                coupon.Status = CouponStatus.Completed;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsCouponValidAsync(int couponId)
        {
            var coupon = await _context.Coupons.FindAsync(couponId);
            return coupon != null && 
                   coupon.Status == CouponStatus.Active && 
                   coupon.ExpiryDate > DateTime.UtcNow && 
                   coupon.UsedServices < coupon.TotalServices;
        }
    }
}
