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
            try
            {
                var serviceCenter = new ServiceCenter
                {
                    Name = createDto.Name,
                    Address = createDto.Address,
                    ContactNumber = createDto.ContactNumber
                };

                var createdServiceCenter = await _serviceCenterService.CreateServiceCenterAsync(serviceCenter);
                
                var response = new ServiceCenterDto
                {
                    Id = createdServiceCenter.Id,
                    Name = createdServiceCenter.Name,
                    Address = createdServiceCenter.Address,
                    ContactNumber = createdServiceCenter.ContactNumber
                };

                return CreatedAtAction(nameof(GetServiceCenterById), new { id = createdServiceCenter.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating service center: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceCenterById(int id)
        {
            var serviceCenter = await _serviceCenterService.GetServiceCenterByIdAsync(id);
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
                var serviceCenters = await _serviceCenterService.GetAllServiceCentersAsync();
                
                var response = serviceCenters.Select(sc => new ServiceCenterDto
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    Address = sc.Address,
                    ContactNumber = sc.ContactNumber
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving service centers: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceCenter(int id, UpdateServiceCenterDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingServiceCenter = await _serviceCenterService.GetServiceCenterByIdAsync(id);
                if (existingServiceCenter == null)
                    return NotFound($"Service center with ID {id} not found");

                var serviceCenter = new ServiceCenter
                {
                    Id = updateDto.Id,
                    Name = updateDto.Name,
                    Address = updateDto.Address,
                    ContactNumber = updateDto.ContactNumber
                };

                var updatedServiceCenter = await _serviceCenterService.UpdateServiceCenterAsync(serviceCenter);
                
                var response = new ServiceCenterDto
                {
                    Id = updatedServiceCenter.Id,
                    Name = updatedServiceCenter.Name,
                    Address = updatedServiceCenter.Address,
                    ContactNumber = updatedServiceCenter.ContactNumber
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating service center: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceCenter(int id)
        {
            try
            {
                var result = await _serviceCenterService.DeleteServiceCenterAsync(id);
                if (!result)
                    return NotFound($"Service center with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting service center: {ex.Message}");
            }
        }
    }
}
