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
            ArgumentNullException.ThrowIfNull(createDto);
            
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

                var createdCustomer = await _customerService.CreateCustomerAsync(customer).ConfigureAwait(false);
                var response = MapToDto(createdCustomer);

                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid customer data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Customer operation failed: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id).ConfigureAwait(false);
            if (customer == null)
                return NotFound($"Customer with ID {id} not found");

            var response = MapToDto(customer);
            return Ok(response);
        }

        [HttpGet("mobile/{mobileNumber}")]
        public async Task<IActionResult> GetCustomerByMobile(string mobileNumber)
        {
            var customer = await _customerService.GetCustomerByMobileAsync(mobileNumber).ConfigureAwait(false);
            if (customer == null)
                return NotFound($"Customer with mobile number {mobileNumber} not found");

            var response = MapToDto(customer);
            return Ok(response);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            var customer = await _customerService.GetCustomerByEmailAsync(email).ConfigureAwait(false);
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
                var customers = await _customerService.GetAllCustomersAsync().ConfigureAwait(false);
                var response = customers.Select(MapToDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Customer retrieval failed: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);
            
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingCustomer = await _customerService.GetCustomerByIdAsync(id).ConfigureAwait(false);
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

                var updatedCustomer = await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);
                var response = MapToDto(updatedCustomer);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid customer data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Customer operation failed: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id).ConfigureAwait(false);
                if (!result)
                    return NotFound($"Customer with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Customer deletion failed: {ex.Message}");
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateCustomer(int id)
        {
            try
            {
                var result = await _customerService.ActivateCustomerAsync(id).ConfigureAwait(false);
                if (!result)
                    return NotFound($"Customer with ID {id} not found");

                return Ok(new { message = "Customer activated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Customer activation failed: {ex.Message}");
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeactivateCustomerAsync(id).ConfigureAwait(false);
                if (!result)
                    return NotFound($"Customer with ID {id} not found");

                return Ok(new { message = "Customer deactivated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Customer deactivation failed: {ex.Message}");
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


