using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CouponHub.DataAccess;
using CouponHub.DataAccess.Models;
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

        public AuthController(CouponHubDbContext context)
        {
            _context = context;
        }

        // ============================
        // 1. Email + Password (Admin / SuperAdmin)
        // ============================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized(new { error = "Invalid email or password" });

            // Debug: Let's see what we're comparing
            var inputPasswordHash = HashPassword(dto.Password);
            var storedHash = user.PasswordHash;
            
            Console.WriteLine($"Input password: {dto.Password}");
            Console.WriteLine($"Input hash: {inputPasswordHash}");
            Console.WriteLine($"Stored hash: {storedHash}");
            Console.WriteLine($"Hashes match: {inputPasswordHash == storedHash}");

            if (!VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized(new { error = "Invalid email or password" });

            return Ok(new { message = "Login successful", role = user.Role, user });
        }

        // ============================
        // 2. Request OTP (Customer)
        // ============================
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequest dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.MobileNumber == dto.MobileNumber);

            if (customer == null)
            {
                // create new customer if not exists
                customer = new Customer
                {
                    MobileNumber = dto.MobileNumber,
                    FirstName = "", // Will be filled during OTP verification
                    LastName = "",
                    Email = ""
                };
                _context.Customers.Add(customer);
            }

            // generate OTP (for demo: fixed or random)
            var otp = new Random().Next(1000, 9999).ToString();
            customer.OtpCode = otp;
            customer.OtpExpiry = DateTime.UtcNow.AddMinutes(5);

            await _context.SaveChangesAsync();

            // TODO: send OTP via SMS gateway (Twilio/Firebase)
            return Ok(new { message = "OTP sent", otp = otp }); // ⚠️ return OTP for demo only
        }

        // ============================
        // 3. Verify OTP (Customer)
        // ============================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.MobileNumber == dto.MobileNumber);

            if (customer == null || customer.OtpCode != dto.Otp || customer.OtpExpiry < DateTime.UtcNow)
                return Unauthorized(new { error = "Invalid or expired OTP" });

            // clear OTP after use
            customer.OtpCode = null;
            customer.OtpExpiry = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "OTP login successful", role = "Customer", customer });
        }

        // ============================
        // 4. Google Login (Customer)
        // ============================
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.GoogleId == dto.GoogleId);

            if (customer == null)
            {
                customer = new Customer
                {
                    GoogleId = dto.GoogleId,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    MobileNumber = "" // Google login doesn't require mobile number
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Google login successful", role = "Customer", customer });
        }

        // ============================
        // Helpers
        // ============================
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
}
