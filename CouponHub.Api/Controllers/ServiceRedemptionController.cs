using Microsoft.AspNetCore.Mvc;
using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;
using CouponHub.Api.DTOs;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/serviceredemptions")]
    public class ServiceRedemptionController : ControllerBase
    {
        private readonly IServiceRedemptionService _serviceRedemptionService;

        public ServiceRedemptionController(IServiceRedemptionService serviceRedemptionService)
        {
            _serviceRedemptionService = serviceRedemptionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRedemption(CreateServiceRedemptionDto createDto)
        {
            try
            {
                var serviceRedemption = new ServiceRedemption
                {
                    CouponId = createDto.CouponId,
                    ServiceCenterId = createDto.ServiceCenterId,
                    CustomerId = createDto.CustomerId,
                    Notes = createDto.Notes,
                    InvoiceId = createDto.InvoiceId,
                    RedemptionDate = DateTime.UtcNow
                };

                var createdServiceRedemption = await _serviceRedemptionService.CreateServiceRedemptionAsync(serviceRedemption);
                var response = MapToDto(createdServiceRedemption);

                return CreatedAtAction(nameof(GetServiceRedemptionById), new { id = createdServiceRedemption.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating service redemption: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRedemptionById(int id)
        {
            var serviceRedemption = await _serviceRedemptionService.GetServiceRedemptionByIdAsync(id);
            if (serviceRedemption == null)
                return NotFound($"Service redemption with ID {id} not found");

            var response = MapToDto(serviceRedemption);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServiceRedemptions()
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetAllServiceRedemptionsAsync();
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving service redemptions: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetServiceRedemptionsByCustomerId(int customerId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByCustomerIdAsync(customerId);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving service redemptions for customer: {ex.Message}");
            }
        }

        [HttpGet("servicecenter/{serviceCenterId}")]
        public async Task<IActionResult> GetServiceRedemptionsByServiceCenterId(int serviceCenterId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByServiceCenterIdAsync(serviceCenterId);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving service redemptions for service center: {ex.Message}");
            }
        }

        [HttpGet("coupon/{couponId}")]
        public async Task<IActionResult> GetServiceRedemptionsByCouponId(int couponId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByCouponIdAsync(couponId);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving service redemptions for coupon: {ex.Message}");
            }
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetServiceRedemptionsByInvoiceId(int invoiceId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByInvoiceIdAsync(invoiceId);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving service redemptions for invoice: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRedemption(int id, UpdateServiceRedemptionDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingServiceRedemption = await _serviceRedemptionService.GetServiceRedemptionByIdAsync(id);
                if (existingServiceRedemption == null)
                    return NotFound($"Service redemption with ID {id} not found");

                var serviceRedemption = new ServiceRedemption
                {
                    Id = updateDto.Id,
                    CouponId = updateDto.CouponId,
                    ServiceCenterId = updateDto.ServiceCenterId,
                    CustomerId = updateDto.CustomerId,
                    Notes = updateDto.Notes,
                    InvoiceId = updateDto.InvoiceId,
                    RedemptionDate = existingServiceRedemption.RedemptionDate
                };

                var updatedServiceRedemption = await _serviceRedemptionService.UpdateServiceRedemptionAsync(serviceRedemption);
                var response = MapToDto(updatedServiceRedemption);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating service redemption: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRedemption(int id)
        {
            try
            {
                var result = await _serviceRedemptionService.DeleteServiceRedemptionAsync(id);
                if (!result)
                    return NotFound($"Service redemption with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting service redemption: {ex.Message}");
            }
        }

        private static ServiceRedemptionDto MapToDto(ServiceRedemption serviceRedemption)
        {
            return new ServiceRedemptionDto
            {
                Id = serviceRedemption.Id,
                CouponId = serviceRedemption.CouponId,
                CouponCode = serviceRedemption.Coupon?.CouponCode ?? string.Empty,
                ServiceCenterId = serviceRedemption.ServiceCenterId,
                ServiceCenterName = serviceRedemption.ServiceCenter?.Name ?? string.Empty,
                CustomerId = serviceRedemption.CustomerId,
                CustomerName = $"{serviceRedemption.Customer?.FirstName} {serviceRedemption.Customer?.LastName}".Trim(),
                RedemptionDate = serviceRedemption.RedemptionDate,
                Notes = serviceRedemption.Notes,
                InvoiceId = serviceRedemption.InvoiceId,
                InvoiceNumber = serviceRedemption.Invoice?.InvoiceNumber
            };
        }
    }
}


