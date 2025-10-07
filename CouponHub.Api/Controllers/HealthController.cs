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
        public async Task<IActionResult> Get()
        {
            try
            {
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                
                return Ok(new
                {
                    Status = canConnect ? "Healthy" : "Degraded",
                    Message = canConnect ? "CouponHub API is running successfully!" : "API is running but database connection failed",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    Database = canConnect ? "Connected" : "Disconnected"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return Ok(new
                {
                    Status = "Degraded",
                    Message = "API is running but health check failed",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    Error = ex.Message
                });
            }
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
