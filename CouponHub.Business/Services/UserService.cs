using CouponHub.Business.Interfaces;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouponHub.Business.Services
{
    public class UserService : IUserService
    {
        private readonly CouponHubDbContext _context;
        private readonly IPasswordResetTokenService _passwordResetTokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;

        public UserService(CouponHubDbContext context, IPasswordResetTokenService passwordResetTokenService, IEmailService emailService, ILogger<UserService> logger)
        {
            _context = context;
            _passwordResetTokenService = passwordResetTokenService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Validate Admin role requires ServiceCenterId
            if (user.Role == "Admin" && !user.ServiceCenterId.HasValue)
            {
                throw new ArgumentException("ServiceCenterId is required for Admin users");
            }
            
            // Validate SuperAdmin should not have ServiceCenterId
            if (user.Role == "SuperAdmin" && user.ServiceCenterId.HasValue)
            {
                throw new ArgumentException("SuperAdmin users cannot be assigned to a Service Center");
            }
            
            // Set password as empty for new users
            user.PasswordHash = string.Empty;
            
            // Ensure SuperAdmin users have null ServiceCenterId
            if (user.Role == "SuperAdmin")
            {
                user.ServiceCenterId = null;
            }
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate password setup token for Admin users
            if (user.Role == "Admin")
            {
                var token = await _passwordResetTokenService.GeneratePasswordSetupTokenAsync(user.Id);
                
                // Send password setup email
                var emailSent = await _emailService.SendPasswordSetupEmailAsync(user, token);
                
                if (emailSent)
                {
                    _logger.LogInformation("Password setup email sent successfully to {Email}", user.Email);
                }
                else
                {
                    _logger.LogWarning("Failed to send password setup email to {Email}. Token: {Token}", user.Email, token);
                    // Fallback: log token for manual distribution
                    Console.WriteLine($"Password setup token for user {user.Email}: {token}");
                }
            }

            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.ServiceCenter)
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if (user != null)
            {
                user.ServiceCenterName = user.ServiceCenter?.Name;
            }
            
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.ServiceCenter)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                user.ServiceCenterName = user.ServiceCenter?.Name;
            }

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersListAsync(bool allUsers = false)
        {
            var query = _context.Users
                .Include(u => u.ServiceCenter)
                .AsQueryable();

            // Filter out soft-deleted users unless allUsers is true
            if (!allUsers)
            {
                query = query.Where(u => !u.DelStatus);
            }

            // Filter active users unless allUsers is true
            if (!allUsers)
            {
                query = query.Where(u => u.IsActive);
            }

            var users = await query
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            // Populate ServiceCenterName for each user
            foreach (var user in users)
            {
                user.ServiceCenterName = user.ServiceCenter?.Name;
            }

            return users;
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string? searchTerm = null, bool allUsers = false)
        {
            var query = _context.Users
                .Include(u => u.ServiceCenter)
                .AsQueryable();

            // Filter out soft-deleted users unless allUsers is true
            if (!allUsers)
            {
                query = query.Where(u => !u.DelStatus);
            }

            // Filter active users unless allUsers is true
            if (!allUsers)
            {
                query = query.Where(u => u.IsActive);
            }

            // Apply search term filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(u => 
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchLower)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchLower)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                    (u.MobileNumber != null && u.MobileNumber.Contains(searchTerm)) ||
                    (u.ServiceCenter != null && u.ServiceCenter.Name != null && u.ServiceCenter.Name.ToLower().Contains(searchLower))
                );
            }

            var users = await query
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            // Populate ServiceCenterName for each user
            foreach (var user in users)
            {
                user.ServiceCenterName = user.ServiceCenter?.Name;
            }

            return users;
        }

        public async Task<User?> UpdateUserAsync(int userId, User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null)
            {
                return null;
            }

            // Validate Admin role requires ServiceCenterId
            if (updatedUser.Role == "Admin" && !updatedUser.ServiceCenterId.HasValue)
            {
                throw new ArgumentException("ServiceCenterId is required for Admin users");
            }
            
            // Validate SuperAdmin should not have ServiceCenterId
            if (updatedUser.Role == "SuperAdmin" && updatedUser.ServiceCenterId.HasValue)
            {
                throw new ArgumentException("SuperAdmin users cannot be assigned to a Service Center");
            }

            // Update user properties
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.MobileNumber = updatedUser.MobileNumber;
            existingUser.Role = updatedUser.Role;
            
            // Handle ServiceCenterId based on role
            if (updatedUser.Role == "SuperAdmin")
            {
                // SuperAdmin should always have null ServiceCenterId
                existingUser.ServiceCenterId = null;
            }
            else
            {
                // For other roles, use the provided ServiceCenterId
                existingUser.ServiceCenterId = updatedUser.ServiceCenterId;
            }
            
            existingUser.IsActive = updatedUser.IsActive;

            // Only update password if provided
            if (!string.IsNullOrEmpty(updatedUser.PasswordHash))
            {
                existingUser.PasswordHash = updatedUser.PasswordHash;
            }

            await _context.SaveChangesAsync();
            
            // Load with ServiceCenter to populate ServiceCenterName
            await _context.Entry(existingUser)
                .Reference(u => u.ServiceCenter)
                .LoadAsync();
            
            existingUser.ServiceCenterName = existingUser.ServiceCenter?.Name;
            
            return existingUser;
        }

        public async Task<bool> ChangeUserStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Prevent changing status of SuperAdmin users
            if (user.Role == "SuperAdmin")
            {
                throw new ArgumentException("Cannot change status of SuperAdmin users");
            }

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            
            var statusAction = isActive ? "activated" : "deactivated";
            _logger.LogInformation("User {UserId} ({Email}) has been {StatusAction}", user.Id, user.Email, statusAction);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Prevent deleting SuperAdmin users
            if (user.Role == "SuperAdmin")
            {
                throw new ArgumentException("Cannot delete SuperAdmin users");
            }

            // Perform soft delete by setting DelStatus to true
            user.DelStatus = true;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User {UserId} ({Email}) has been deleted", user.Id, user.Email);
            return true;
        }
    }
}
