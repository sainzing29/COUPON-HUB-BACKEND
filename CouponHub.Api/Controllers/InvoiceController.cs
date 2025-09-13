using Microsoft.AspNetCore.Mvc;
using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;
using CouponHub.Api.DTOs;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(CreateInvoiceDto createDto)
        {
            try
            {
                var invoice = new Invoice
                {
                    InvoiceNumber = GenerateInvoiceNumber(),
                    CustomerId = createDto.CustomerId,
                    ServiceCenterId = createDto.ServiceCenterId,
                    CouponId = createDto.CouponId,
                    SubTotal = createDto.SubTotal,
                    TaxAmount = createDto.TaxAmount,
                    DiscountAmount = createDto.DiscountAmount,
                    TotalAmount = createDto.TotalAmount,
                    Currency = createDto.Currency,
                    PaymentMethod = createDto.PaymentMethod,
                    PaymentStatus = createDto.PaymentStatus,
                    Notes = createDto.Notes
                };

                var createdInvoice = await _invoiceService.CreateInvoiceAsync(invoice);
                var response = MapToDto(createdInvoice);

                return CreatedAtAction(nameof(GetInvoiceById), new { id = createdInvoice.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating invoice: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound($"Invoice with ID {id} not found");

            var response = MapToDto(invoice);
            return Ok(response);
        }

        [HttpGet("number/{invoiceNumber}")]
        public async Task<IActionResult> GetInvoiceByNumber(string invoiceNumber)
        {
            var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);
            if (invoice == null)
                return NotFound($"Invoice with number {invoiceNumber} not found");

            var response = MapToDto(invoice);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetAllInvoicesAsync();
                var response = invoices.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving invoices: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetInvoicesByCustomerId(int customerId)
        {
            try
            {
                var invoices = await _invoiceService.GetInvoicesByCustomerIdAsync(customerId);
                var response = invoices.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving invoices for customer: {ex.Message}");
            }
        }

        [HttpGet("servicecenter/{serviceCenterId}")]
        public async Task<IActionResult> GetInvoicesByServiceCenterId(int serviceCenterId)
        {
            try
            {
                var invoices = await _invoiceService.GetInvoicesByServiceCenterIdAsync(serviceCenterId);
                var response = invoices.Select(MapToDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving invoices for service center: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, UpdateInvoiceDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID mismatch");

                var existingInvoice = await _invoiceService.GetInvoiceByIdAsync(id);
                if (existingInvoice == null)
                    return NotFound($"Invoice with ID {id} not found");

                var invoice = new Invoice
                {
                    Id = updateDto.Id,
                    InvoiceNumber = existingInvoice.InvoiceNumber, // Keep original invoice number
                    CustomerId = updateDto.CustomerId,
                    ServiceCenterId = updateDto.ServiceCenterId,
                    CouponId = updateDto.CouponId,
                    SubTotal = updateDto.SubTotal,
                    TaxAmount = updateDto.TaxAmount,
                    DiscountAmount = updateDto.DiscountAmount,
                    TotalAmount = updateDto.TotalAmount,
                    Currency = updateDto.Currency,
                    PaymentMethod = updateDto.PaymentMethod,
                    PaymentStatus = updateDto.PaymentStatus,
                    Notes = updateDto.Notes,
                    CreatedAt = existingInvoice.CreatedAt,
                    IsDeleted = existingInvoice.IsDeleted
                };

                var updatedInvoice = await _invoiceService.UpdateInvoiceAsync(invoice);
                var response = MapToDto(updatedInvoice);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating invoice: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var result = await _invoiceService.DeleteInvoiceAsync(id);
                if (!result)
                    return NotFound($"Invoice with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting invoice: {ex.Message}");
            }
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerId = invoice.CustomerId,
                CustomerName = $"{invoice.Customer?.FirstName} {invoice.Customer?.LastName}".Trim(),
                ServiceCenterId = invoice.ServiceCenterId,
                ServiceCenterName = invoice.ServiceCenter?.Name ?? string.Empty,
                CouponId = invoice.CouponId,
                CouponCode = invoice.Coupon?.CouponCode,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                DiscountAmount = invoice.DiscountAmount,
                TotalAmount = invoice.TotalAmount,
                Currency = invoice.Currency,
                PaymentMethod = invoice.PaymentMethod,
                PaymentStatus = invoice.PaymentStatus,
                PaidAt = invoice.PaidAt,
                CreatedAt = invoice.CreatedAt,
                Notes = invoice.Notes
            };
        }

        private static string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyy}-{DateTime.UtcNow.Ticks % 10000:D4}";
        }
    }
}


