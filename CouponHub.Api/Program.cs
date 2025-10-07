using CouponHub.Api;

var builder = WebApplication.CreateBuilder(args);

// Enable environment variable substitution
builder.Configuration.AddEnvironmentVariables();

// Configure services
var startup = new Startup(builder.Configuration);
try
{
    startup.ConfigureServices(builder.Services);
    Console.WriteLine("✅ Services configured successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error configuring services: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}

WebApplication app;
try
{
    app = builder.Build();
    Console.WriteLine("✅ Application built successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error building application: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}

// Configure the HTTP request pipeline
try
{
    startup.Configure(app, app.Environment);
    Console.WriteLine("✅ Application configured successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error configuring application: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}

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
app.MapGet("/health", () => 
{
    return Results.Ok(new
    {
        Status = "Healthy",
        Message = "CouponHub API is running successfully!",
        Timestamp = DateTime.UtcNow,
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
        Port = Environment.GetEnvironmentVariable("PORT") ?? "Unknown",
        Platform = "Railway"
    });
});

Console.WriteLine("🚀 CouponHub API is starting...");
Console.WriteLine($"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}");
Console.WriteLine($"Port: {Environment.GetEnvironmentVariable("PORT") ?? "Unknown"}");
Console.WriteLine($"Database URL: {Environment.GetEnvironmentVariable("DATABASE_URL")?.Substring(0, 20) ?? "Not set"}...");
Console.WriteLine($"JWT Key: {Environment.GetEnvironmentVariable("JWT_KEY")?.Substring(0, 10) ?? "Not set"}...");
Console.WriteLine("✅ Application is ready to accept requests");

app.Run();

