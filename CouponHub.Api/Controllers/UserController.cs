using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "SuperAdmin")]
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
            try
            {
                var createdUser = await _userService.CreateUserAsync(user).ConfigureAwait(false);
                return Ok(createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUsersList([FromQuery] bool allUsers = false)
        {
            var users = await _userService.GetUsersListAsync(allUsers).ConfigureAwait(false);
            return Ok(users);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? searchTerm = null, [FromQuery] bool allUsers = false)
        {
            var users = await _userService.SearchUsersAsync(searchTerm, allUsers).ConfigureAwait(false);
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId).ConfigureAwait(false);
            
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            return Ok(user);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(userId, user).ConfigureAwait(false);
                
                if (updatedUser == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{userId}/status")]
        public async Task<IActionResult> ChangeUserStatus(int userId, [FromBody] ChangeUserStatusRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                var result = await _userService.ChangeUserStatusAsync(userId, request.IsActive).ConfigureAwait(false);
                
                if (!result)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                var statusMessage = request.IsActive ? "activated" : "deactivated";
                return Ok(new { message = $"User {statusMessage} successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId).ConfigureAwait(false);
                
                if (!result)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                return Ok(new { message = "User deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ChangeUserStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
