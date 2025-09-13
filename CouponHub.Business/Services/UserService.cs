using CouponHub.Business.Interfaces;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponHub.Business.Services
{
    public class UserService : IUserService
    {
        private readonly CouponHubDbContext _context;

        public UserService(CouponHubDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByMobileAsync(string mobile)
        {
            var user = await _context.Users
                .Include(u => u.ServiceCenter)
                .FirstOrDefaultAsync(u => u.MobileNumber == mobile);
            
            if (user != null)
            {
                user.ServiceCenterName = user.ServiceCenter?.Name;
            }
            
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersListAsync()
        {
            var users = await _context.Users
                .Include(u => u.ServiceCenter)
                .Where(u => u.IsActive)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            // Populate ServiceCenterName for each user
            foreach (var user in users)
            {
                user.ServiceCenterName = user.ServiceCenter?.Name;
            }

            return users;
        }
    }
}
