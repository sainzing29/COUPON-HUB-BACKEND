using Microsoft.AspNetCore.Mvc;
using CouponHub.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly CouponHubDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(CouponHubDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint - no authentication required
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Message = "CouponHub API is running successfully!",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        /// <summary>
        /// Simple ping endpoint for basic connectivity test
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { Message = "Pong!", Timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Test endpoint to verify Railway deployment
        /// </summary>
        [HttpGet("railway")]
        public IActionResult RailwayTest()
        {
            return Ok(new
            {
                Message = "🚀 Railway deployment test successful!",
                Timestamp = DateTime.UtcNow,
                Platform = "Railway",
                Status = "Deployed and Running",
                Environment = Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") ?? "Unknown",
                Port = Environment.GetEnvironmentVariable("PORT") ?? "Unknown"
            });
        }
    }
}
