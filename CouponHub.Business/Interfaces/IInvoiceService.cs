using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);
        Task<IEnumerable<Invoice>> GetInvoicesByServiceCenterIdAsync(int serviceCenterId);
        Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
        Task<bool> DeleteInvoiceAsync(int id);
    }
}


