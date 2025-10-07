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
    try
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
    }
    catch (Exception ex)
    {
        return Results.Ok(new
        {
            Status = "Degraded",
            Message = "API is running but health check had issues",
            Timestamp = DateTime.UtcNow,
            Error = ex.Message
        });
    }
});

Console.WriteLine("🚀 CouponHub API is starting...");
Console.WriteLine($"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}");
Console.WriteLine($"Port: {Environment.GetEnvironmentVariable("PORT") ?? "Unknown"}");
Console.WriteLine("✅ Application is ready to accept requests");

app.Run();

