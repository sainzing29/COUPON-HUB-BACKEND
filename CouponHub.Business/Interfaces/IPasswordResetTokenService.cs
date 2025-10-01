using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface IPasswordResetTokenService
    {
        Task<string> GeneratePasswordSetupTokenAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
        Task<bool> UseTokenAsync(string token, string newPasswordHash);
        Task<User?> GetUserByTokenAsync(string token);
    }
}
