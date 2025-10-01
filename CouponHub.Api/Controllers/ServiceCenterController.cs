using Microsoft.AspNetCore.Mvc;
using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;
using CouponHub.Api.DTOs;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/servicecenters")]
    public class ServiceCenterController : ControllerBase
    {
        private readonly IServiceCenterService _serviceCenterService;

        public ServiceCenterController(IServiceCenterService serviceCenterService)
        {
            _serviceCenterService = serviceCenterService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceCenter(CreateServiceCenterDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);
            
            try
            {
                var serviceCenter = new ServiceCenter
                {
                    Name = createDto.Name,
                    Address = createDto.Address,
                    ContactNumber = createDto.ContactNumber
                };

                var createdServiceCenter = await _serviceCenterService.CreateServiceCenterAsync(serviceCenter).ConfigureAwait(false);
                
                var response = new ServiceCenterDto
                {
                    Id = createdServiceCenter.Id,
                    Name = createdServiceCenter.Name,
                    Address = createdServiceCenter.Address,
                    ContactNumber = createdServiceCenter.ContactNumber
                };

                return CreatedAtAction(nameof(GetServiceCenterById), new { id = createdServiceCenter.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid service center data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service center operation failed: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceCenterById(int id)
        {
            var serviceCenter = await _serviceCenterService.GetServiceCenterByIdAsync(id).ConfigureAwait(false);
            if (serviceCenter == null)
                return NotFound($"Service center with ID {id} not found");

            var response = new ServiceCenterDto
            {
                Id = serviceCenter.Id,
                Name = serviceCenter.Name,
                Address = serviceCenter.Address,
                ContactNumber = serviceCenter.ContactNumber
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServiceCenters()
        {
            try
            {
                var serviceCenters = await _serviceCenterService.GetAllServiceCentersAsync().ConfigureAwait(false);
                
                var response = serviceCenters.Select(sc => new ServiceCenterDto
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    Address = sc.Address,
                    ContactNumber = sc.ContactNumber
                });

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service center retrieval failed: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceCenter(int id, UpdateServiceCenterDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);
            
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingServiceCenter = await _serviceCenterService.GetServiceCenterByIdAsync(id).ConfigureAwait(false);
                if (existingServiceCenter == null)
                    return NotFound($"Service center with ID {id} not found");

                var serviceCenter = new ServiceCenter
                {
                    Id = updateDto.Id,
                    Name = updateDto.Name,
                    Address = updateDto.Address,
                    ContactNumber = updateDto.ContactNumber
                };

                var updatedServiceCenter = await _serviceCenterService.UpdateServiceCenterAsync(serviceCenter).ConfigureAwait(false);
                
                var response = new ServiceCenterDto
                {
                    Id = updatedServiceCenter.Id,
                    Name = updatedServiceCenter.Name,
                    Address = updatedServiceCenter.Address,
                    ContactNumber = updatedServiceCenter.ContactNumber
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid service center data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service center operation failed: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceCenter(int id)
        {
            try
            {
                var result = await _serviceCenterService.DeleteServiceCenterAsync(id).ConfigureAwait(false);
                if (!result)
                    return NotFound($"Service center with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Service center deletion failed: {ex.Message}");
            }
        }
    }
}
