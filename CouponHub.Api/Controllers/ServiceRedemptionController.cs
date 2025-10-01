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
            ArgumentNullException.ThrowIfNull(createDto);
            
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

                var createdServiceRedemption = await _serviceRedemptionService.CreateServiceRedemptionAsync(serviceRedemption).ConfigureAwait(false);
                var response = MapToDto(createdServiceRedemption);

                return CreatedAtAction(nameof(GetServiceRedemptionById), new { id = createdServiceRedemption.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid service redemption data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption operation failed: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRedemptionById(int id)
        {
            var serviceRedemption = await _serviceRedemptionService.GetServiceRedemptionByIdAsync(id).ConfigureAwait(false);
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
                var serviceRedemptions = await _serviceRedemptionService.GetAllServiceRedemptionsAsync().ConfigureAwait(false);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption retrieval failed: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetServiceRedemptionsByCustomerId(int customerId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByCustomerIdAsync(customerId).ConfigureAwait(false);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption retrieval failed: {ex.Message}");
            }
        }

        [HttpGet("servicecenter/{serviceCenterId}")]
        public async Task<IActionResult> GetServiceRedemptionsByServiceCenterId(int serviceCenterId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByServiceCenterIdAsync(serviceCenterId).ConfigureAwait(false);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption retrieval failed: {ex.Message}");
            }
        }

        [HttpGet("coupon/{couponId}")]
        public async Task<IActionResult> GetServiceRedemptionsByCouponId(int couponId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByCouponIdAsync(couponId).ConfigureAwait(false);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption retrieval failed: {ex.Message}");
            }
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetServiceRedemptionsByInvoiceId(int invoiceId)
        {
            try
            {
                var serviceRedemptions = await _serviceRedemptionService.GetServiceRedemptionsByInvoiceIdAsync(invoiceId).ConfigureAwait(false);
                var response = serviceRedemptions.Select(MapToDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption retrieval failed: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRedemption(int id, UpdateServiceRedemptionDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);
            
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingServiceRedemption = await _serviceRedemptionService.GetServiceRedemptionByIdAsync(id).ConfigureAwait(false);
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

                var updatedServiceRedemption = await _serviceRedemptionService.UpdateServiceRedemptionAsync(serviceRedemption).ConfigureAwait(false);
                var response = MapToDto(updatedServiceRedemption);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid service redemption data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption operation failed: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRedemption(int id)
        {
            try
            {
                var result = await _serviceRedemptionService.DeleteServiceRedemptionAsync(id).ConfigureAwait(false);
                if (!result)
                    return NotFound($"Service redemption with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service redemption deletion failed: {ex.Message}");
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


