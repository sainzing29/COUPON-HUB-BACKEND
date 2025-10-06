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

app.Run();

