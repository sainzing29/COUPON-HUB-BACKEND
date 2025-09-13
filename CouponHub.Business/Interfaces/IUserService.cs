using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<User?> GetUserByMobileAsync(string mobile);
        Task<IEnumerable<User>> GetUsersListAsync();
    }
}