using Microsoft.AspNetCore.Mvc;
using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;
using CouponHub.Api.DTOs;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/coupons")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CreateCouponDto createDto)
        {
            try
            {
                var coupon = new Coupon
                {
                    CouponCode = GenerateCouponCode(),
                    CustomerId = createDto.CustomerId,
                    TotalServices = createDto.TotalServices,
                    ExpiryDate = createDto.ExpiryDate,
                    Status = createDto.Status
                };

                var createdCoupon = await _couponService.CreateCouponAsync(coupon);
                var response = MapToDto(createdCoupon);

                return CreatedAtAction(nameof(GetCouponById), new { id = createdCoupon.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating coupon: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
                return NotFound($"Coupon with ID {id} not found");

            var response = MapToDto(coupon);
            return Ok(response);
        }

        [HttpGet("code/{couponCode}")]
        public async Task<IActionResult> GetCouponByCode(string couponCode)
        {
            var coupon = await _couponService.GetCouponByCodeAsync(couponCode);
            if (coupon == null)
                return NotFound($"Coupon with code {couponCode} not found");

            var response = MapToDto(coupon);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            try
            {
                var coupons = await _couponService.GetAllCouponsAsync();
                var response = coupons.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving coupons: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCouponsByCustomerId(int customerId)
        {
            try
            {
                var coupons = await _couponService.GetCouponsByCustomerIdAsync(customerId);
                var response = coupons.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving coupons for customer: {ex.Message}");
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCoupons()
        {
            try
            {
                var coupons = await _couponService.GetActiveCouponsAsync();
                var response = coupons.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving active coupons: {ex.Message}");
            }
        }

        [HttpGet("expired")]
        public async Task<IActionResult> GetExpiredCoupons()
        {
            try
            {
                var coupons = await _couponService.GetExpiredCouponsAsync();
                var response = coupons.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving expired coupons: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, UpdateCouponDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingCoupon = await _couponService.GetCouponByIdAsync(id);
                if (existingCoupon == null)
                    return NotFound($"Coupon with ID {id} not found");

                var coupon = new Coupon
                {
                    Id = updateDto.Id,
                    CouponCode = existingCoupon.CouponCode, // Keep original coupon code
                    CustomerId = updateDto.CustomerId,
                    TotalServices = updateDto.TotalServices,
                    UsedServices = existingCoupon.UsedServices, // Keep current usage
                    ExpiryDate = updateDto.ExpiryDate,
                    Status = updateDto.Status,
                    PurchaseDate = existingCoupon.PurchaseDate
                };

                var updatedCoupon = await _couponService.UpdateCouponAsync(coupon);
                var response = MapToDto(updatedCoupon);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating coupon: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            try
            {
                var result = await _couponService.DeleteCouponAsync(id);
                if (!result)
                    return NotFound($"Coupon with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting coupon: {ex.Message}");
            }
        }

        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemCoupon(RedeemCouponDto redeemDto)
        {
            try
            {
                var result = await _couponService.RedeemCouponAsync(
                    redeemDto.CouponId, 
                    redeemDto.ServiceCenterId, 
                    redeemDto.CustomerId, 
                    redeemDto.Notes);

                if (!result)
                    return BadRequest("Coupon redemption failed. Coupon may be invalid or expired.");

                return Ok(new { message = "Coupon redeemed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error redeeming coupon: {ex.Message}");
            }
        }

        [HttpGet("{id}/validate")]
        public async Task<IActionResult> ValidateCoupon(int id)
        {
            try
            {
                var isValid = await _couponService.IsCouponValidAsync(id);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error validating coupon: {ex.Message}");
            }
        }

        private static CouponDto MapToDto(Coupon coupon)
        {
            var now = DateTime.UtcNow;
            var isValid = coupon.Status == "Active" && 
                         coupon.ExpiryDate > now && 
                         coupon.UsedServices < coupon.TotalServices;

            return new CouponDto
            {
                Id = coupon.Id,
                CouponCode = coupon.CouponCode,
                CustomerId = coupon.CustomerId,
                CustomerName = $"{coupon.Customer?.FirstName} {coupon.Customer?.LastName}".Trim(),
                TotalServices = coupon.TotalServices,
                UsedServices = coupon.UsedServices,
                RemainingServices = coupon.TotalServices - coupon.UsedServices,
                PurchaseDate = coupon.PurchaseDate,
                ExpiryDate = coupon.ExpiryDate,
                Status = coupon.Status,
                IsValid = isValid,
                RedemptionCount = coupon.Redemptions?.Count ?? 0
            };
        }

        private static string GenerateCouponCode()
        {
            return $"CPN-{DateTime.UtcNow.Ticks % 100000:D5}";
        }
    }
}




