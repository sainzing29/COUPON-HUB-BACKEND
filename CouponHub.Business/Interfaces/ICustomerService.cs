using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByMobileAsync(string mobileNumber);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> ActivateCustomerAsync(int id);
        Task<bool> DeactivateCustomerAsync(int id);
    }
}


