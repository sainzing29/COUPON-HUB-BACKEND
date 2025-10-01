using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendPasswordSetupEmailAsync(User user, string token);
        Task<bool> SendPasswordResetEmailAsync(User user, string token);
        Task<bool> SendWelcomeEmailAsync(User user);
    }
}
