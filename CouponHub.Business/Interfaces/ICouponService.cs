using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon> CreateCouponAsync(Coupon coupon);
        Task<Coupon?> GetCouponByIdAsync(int id);
        Task<Coupon?> GetCouponByCodeAsync(string couponCode);
        Task<IEnumerable<Coupon>> GetAllCouponsAsync();
        Task<IEnumerable<Coupon>> GetCouponsByCustomerIdAsync(int customerId);
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();
        Task<IEnumerable<Coupon>> GetExpiredCouponsAsync();
        Task<Coupon> UpdateCouponAsync(Coupon coupon);
        Task<bool> DeleteCouponAsync(int id);
        Task<bool> RedeemCouponAsync(int couponId, int serviceCenterId, int customerId, string notes = "");
        Task<bool> IsCouponValidAsync(int couponId);
    }
}


