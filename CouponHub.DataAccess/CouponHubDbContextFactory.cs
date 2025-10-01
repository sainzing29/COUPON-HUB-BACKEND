using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CouponHub.DataAccess
{
    public class CouponHubDbContextFactory : IDesignTimeDbContextFactory<CouponHubDbContext>
    {
        public CouponHubDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CouponHub.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<CouponHubDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new CouponHubDbContext(optionsBuilder.Options);
        }
    }
}

