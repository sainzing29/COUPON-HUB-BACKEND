using CouponHub.Api;

var builder = WebApplication.CreateBuilder(args);

// Enable environment variable substitution
builder.Configuration.AddEnvironmentVariables();

// Configure services
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

WebApplication app;
try
{
    app = builder.Build();
}
catch (Exception ex)
{
    Console.WriteLine($"Error building application: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}

// Configure the HTTP request pipeline
startup.Configure(app, app.Environment);

// Map controllers
app.MapControllers();

// Add a simple root endpoint for Railway deployment verification
app.MapGet("/", () => new
{
    Message = "🚀 CouponHub API is running successfully!",
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    Platform = "Railway",
    Version = "1.0.0"
});

// Add a simple health endpoint at root level for Railway healthcheck
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Message = "CouponHub API is running successfully!",
    Timestamp = DateTime.UtcNow,
    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
});

app.Run();

