using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
using CouponHub.Business.Services;
using CouponHub.Business.Interfaces;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CouponHubDbContext _context;
        private readonly IUserService _userService;
        private readonly AuthService _authService;
        private readonly IPasswordResetTokenService _passwordResetTokenService;
        private readonly IEmailService _emailService;

        public AuthController(CouponHubDbContext context, IUserService userService, AuthService authService, IPasswordResetTokenService passwordResetTokenService, IEmailService emailService)
        {
            _context = context;
            _userService = userService;
            _authService = authService;
            _passwordResetTokenService = passwordResetTokenService;
            _emailService = emailService;
        }

        // ============================
        // 1. Email + Password (Admin / SuperAdmin)
        // ============================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userService.GetUserByEmailAsync(req.Email).ConfigureAwait(false);
            if (user == null) return Unauthorized("Invalid email or password");

            if (string.IsNullOrEmpty(user.PasswordHash) || 
                !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

       
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        [HttpPost("generate-hash")]
        public IActionResult GenerateHash([FromBody] string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(new { password, hash });
        }

        [HttpPost("test-hash")]
        public IActionResult TestHash([FromBody] string password)
        {
            var hash = HashPassword(password);
            return Ok(new { password, hash });
        }

        // ============================
        // Password Setup Endpoints
        // ============================
        [HttpPost("validate-setup-token")]
        public async Task<IActionResult> ValidateSetupToken([FromBody] ValidateTokenRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var isValid = await _passwordResetTokenService.ValidateTokenAsync(request.Token).ConfigureAwait(false);
            
            if (!isValid)
            {
                return BadRequest("Invalid or expired token");
            }

            var user = await _passwordResetTokenService.GetUserByTokenAsync(request.Token).ConfigureAwait(false);
            
            return Ok(new { 
                isValid = true, 
                user = new { 
                    id = user?.Id, 
                    email = user?.Email, 
                    firstName = user?.FirstName,
                    lastName = user?.LastName 
                } 
            });
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters long");
            }

            var passwordHash = HashPassword(request.Password);
            var success = await _passwordResetTokenService.UseTokenAsync(request.Token, passwordHash).ConfigureAwait(false);

            if (!success)
            {
                return BadRequest("Invalid or expired token");
            }

            return Ok(new { message = "Password set successfully" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await _userService.GetUserByEmailAsync(request.Email).ConfigureAwait(false);
            
            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return Ok(new { message = "If the email exists, a password reset link has been sent." });
            }

            var token = await _passwordResetTokenService.GeneratePasswordSetupTokenAsync(user.Id).ConfigureAwait(false);
            var emailSent = await _emailService.SendPasswordResetEmailAsync(user, token).ConfigureAwait(false);

            if (emailSent)
            {
                return Ok(new { message = "Password reset link has been sent to your email." });
            }
            else
            {
                return BadRequest(new { message = "Failed to send password reset email. Please try again later." });
            }
        }
    }

    // DTOs
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class OtpRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class OtpVerifyRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class GoogleLoginRequest
    {
        public string GoogleId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class ValidateTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class SetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
