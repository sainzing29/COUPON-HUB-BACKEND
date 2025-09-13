using Microsoft.AspNetCore.Mvc;
using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;
using CouponHub.Api.DTOs;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto createDto)
        {
            try
            {
                var customer = new Customer
                {
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    Email = createDto.Email,
                    MobileNumber = createDto.MobileNumber,
                    GoogleId = createDto.GoogleId
                };

                var createdCustomer = await _customerService.CreateCustomerAsync(customer);
                var response = MapToDto(createdCustomer);

                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating customer: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with ID {id} not found");

            var response = MapToDto(customer);
            return Ok(response);
        }

        [HttpGet("mobile/{mobileNumber}")]
        public async Task<IActionResult> GetCustomerByMobile(string mobileNumber)
        {
            var customer = await _customerService.GetCustomerByMobileAsync(mobileNumber);
            if (customer == null)
                return NotFound($"Customer with mobile number {mobileNumber} not found");

            var response = MapToDto(customer);
            return Ok(response);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            var customer = await _customerService.GetCustomerByEmailAsync(email);
            if (customer == null)
                return NotFound($"Customer with email {email} not found");

            var response = MapToDto(customer);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                var response = customers.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving customers: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingCustomer = await _customerService.GetCustomerByIdAsync(id);
                if (existingCustomer == null)
                    return NotFound($"Customer with ID {id} not found");

                var customer = new Customer
                {
                    Id = updateDto.Id,
                    FirstName = updateDto.FirstName,
                    LastName = updateDto.LastName,
                    Email = updateDto.Email,
                    MobileNumber = updateDto.MobileNumber,
                    GoogleId = updateDto.GoogleId,
                    CreatedAt = existingCustomer.CreatedAt,
                    IsActive = existingCustomer.IsActive
                };

                var updatedCustomer = await _customerService.UpdateCustomerAsync(customer);
                var response = MapToDto(updatedCustomer);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating customer: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (!result)
                    return NotFound($"Customer with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting customer: {ex.Message}");
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateCustomer(int id)
        {
            try
            {
                var result = await _customerService.ActivateCustomerAsync(id);
                if (!result)
                    return NotFound($"Customer with ID {id} not found");

                return Ok(new { message = "Customer activated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error activating customer: {ex.Message}");
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeactivateCustomerAsync(id);
                if (!result)
                    return NotFound($"Customer with ID {id} not found");

                return Ok(new { message = "Customer deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deactivating customer: {ex.Message}");
            }
        }

        private static CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                FullName = $"{customer.FirstName} {customer.LastName}".Trim(),
                Email = customer.Email,
                MobileNumber = customer.MobileNumber,
                GoogleId = customer.GoogleId,
                CreatedAt = customer.CreatedAt,
                IsActive = customer.IsActive,
                CouponCount = customer.Coupons?.Count ?? 0,
                InvoiceCount = customer.Invoices?.Count ?? 0
            };
        }
    }
}


