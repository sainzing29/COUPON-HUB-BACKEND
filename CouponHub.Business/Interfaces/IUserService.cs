using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);

        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetUsersListAsync(bool allUsers = false);
        Task<IEnumerable<User>> SearchUsersAsync(string? searchTerm = null, bool allUsers = false);
        Task<User?> UpdateUserAsync(int userId, User updatedUser);
        
        Task<bool> ChangeUserStatusAsync(int userId, bool isActive);
        Task<bool> DeleteUserAsync(int userId);
    }
}