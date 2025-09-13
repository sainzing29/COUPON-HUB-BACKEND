using CouponHub.Business.Interfaces;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponHub.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CouponHubDbContext _context;

        public CustomerService(CouponHubDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Coupons)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetCustomerByMobileAsync(string mobileNumber)
        {
            return await _context.Customers
                .Include(c => c.Coupons)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.MobileNumber == mobileNumber);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers
                .Include(c => c.Coupons)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Include(c => c.Coupons)
                .Include(c => c.Invoices)
                .Where(c => c.IsActive)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            // Soft delete
            customer.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            customer.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            customer.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


