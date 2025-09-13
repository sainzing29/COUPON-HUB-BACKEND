using Microsoft.AspNetCore.Mvc;
using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return Ok(createdUser);
        }

        [HttpGet("{mobile}")]
        public async Task<IActionResult> GetUserByMobile(string mobile)
        {
            var user = await _userService.GetUserByMobileAsync(mobile);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersList()
        {
            var users = await _userService.GetUsersListAsync();
            return Ok(users);
        }
    }
}
