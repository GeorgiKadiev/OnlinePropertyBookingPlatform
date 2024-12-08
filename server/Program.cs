using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetEnv; // Import for loading .env files
using System;
using OnlinePropertyBookingPlatform;
using OnlinePropertyBookingPlatform.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using OnlinePropertyBookingPlatform.Utility; // Include the namespace for CrudRepository
public class Program
{ 

public static void Main(string[] args)
{

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<OnlinePropertyBookingPlatform.Utility.IEmailSender, EmailSender>();
Env.Load();
// Add services to the container
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Add Console logging
builder.Logging.AddDebug();

builder.Services.AddControllers();

try
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Database connection string is not set in environment variables.");
    }

    builder.Services.AddDbContext<PropertyManagementContext>(options =>
        options.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 40))
        ));

    builder.Services.AddScoped(typeof(CrudRepository<>));
}
catch (Exception ex)
{
    var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger("Startup");
    logger.LogError(ex, "An error occurred while configuring the database connection.");
    throw;
}

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "An error occurred during application startup.");
    throw;
}
}

}