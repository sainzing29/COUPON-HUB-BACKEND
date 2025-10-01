using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using CouponHub.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CouponHub.Business.Services
{
    public class PasswordResetTokenService : IPasswordResetTokenService
    {
        private readonly CouponHubDbContext _context;

        public PasswordResetTokenService(CouponHubDbContext context)
        {
            _context = context;
        }

        public async Task<string> GeneratePasswordSetupTokenAsync(int userId)
        {
            // Generate a secure random token
            var token = GenerateSecureToken();
            
            // Set expiry to 7 days from now
            var expiryDate = DateTime.UtcNow.AddDays(7);

            // Invalidate any existing tokens for this user
            await InvalidateExistingTokensAsync(userId);

            // Create new token
            var passwordResetToken = new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                ExpiryDate = expiryDate,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordResetTokens.Add(passwordResetToken);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var passwordResetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(prt => prt.Token == token && !prt.IsUsed);

            if (passwordResetToken == null)
                return false;

            // Check if token is expired
            if (passwordResetToken.ExpiryDate < DateTime.UtcNow)
            {
                // Mark as used to prevent future attempts
                passwordResetToken.IsUsed = true;
                await _context.SaveChangesAsync();
                return false;
            }

            return true;
        }

        public async Task<bool> UseTokenAsync(string token, string newPasswordHash)
        {
            var passwordResetToken = await _context.PasswordResetTokens
                .Include(prt => prt.User)
                .FirstOrDefaultAsync(prt => prt.Token == token && !prt.IsUsed);

            if (passwordResetToken == null)
                return false;

            // Check if token is expired
            if (passwordResetToken.ExpiryDate < DateTime.UtcNow)
            {
                passwordResetToken.IsUsed = true;
                await _context.SaveChangesAsync();
                return false;
            }

            // Update user's password
            passwordResetToken.User.PasswordHash = newPasswordHash;

            // Mark token as used
            passwordResetToken.IsUsed = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserByTokenAsync(string token)
        {
            var passwordResetToken = await _context.PasswordResetTokens
                .Include(prt => prt.User)
                .FirstOrDefaultAsync(prt => prt.Token == token && !prt.IsUsed);

            if (passwordResetToken == null || passwordResetToken.ExpiryDate < DateTime.UtcNow)
                return null;

            return passwordResetToken.User;
        }

        private async Task InvalidateExistingTokensAsync(int userId)
        {
            var existingTokens = await _context.PasswordResetTokens
                .Where(prt => prt.UserId == userId && !prt.IsUsed)
                .ToListAsync();

            foreach (var token in existingTokens)
            {
                token.IsUsed = true;
            }
        }

        private string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
