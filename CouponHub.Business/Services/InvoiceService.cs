using CouponHub.Business.Interfaces;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponHub.Business.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly CouponHubDbContext _context;

        public InvoiceService(CouponHubDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.ServiceCenter)
                .Include(i => i.Coupon)
                .Include(i => i.ServiceRedemptions)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.ServiceCenter)
                .Include(i => i.Coupon)
                .Include(i => i.ServiceRedemptions)
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.ServiceCenter)
                .Include(i => i.Coupon)
                .Where(i => !i.IsDeleted)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.ServiceCenter)
                .Include(i => i.Coupon)
                .Where(i => i.CustomerId == customerId && !i.IsDeleted)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByServiceCenterIdAsync(int serviceCenterId)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.ServiceCenter)
                .Include(i => i.Coupon)
                .Where(i => i.ServiceCenterId == serviceCenterId && !i.IsDeleted)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            // Soft delete
            invoice.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


