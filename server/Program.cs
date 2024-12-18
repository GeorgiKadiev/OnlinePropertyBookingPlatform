using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetEnv; // Import for loading .env files
using System;
using OnlinePropertyBookingPlatform;
using OnlinePropertyBookingPlatform.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using OnlinePropertyBookingPlatform.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using System.Threading.RateLimiting; // Include the namespace for CrudRepository
public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Регистриране на services
        builder.Services.AddControllers();

        // Регистрация на IEmailSender
        builder.Services.AddScoped<OnlinePropertyBookingPlatform.Utility.IEmailSender, EmailSender>();

        // CSRF Middleware
        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        // Rate Limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Request.Path.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    }));
        });

        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["jwt"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                }
            };
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

        // Конфигурация на базата данни
        Env.Load();
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Database connection string is not set in environment variables.");
        }

        builder.Services.AddDbContext<PropertyManagementContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 40))));

        builder.Services.AddScoped(typeof(CrudRepository<>));

        // Създаване на приложението
        var app = builder.Build();

        // Middleware за CSRF токен
        app.Use(async (context, next) =>
        {
            var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
            var tokens = antiforgery.GetAndStoreTokens(context);
            if (tokens.RequestToken != null)
            {
                context.Response.Headers.Append("X-CSRF-TOKEN", tokens.RequestToken);
            }
            await next();
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

       // app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // Rate Limiting Middleware
        app.UseRateLimiter();

        app.MapControllers();

        // Стартиране на приложението
        app.Run();
    }
    /*
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        builder.Services.AddScoped<OnlinePropertyBookingPlatform.Utility.IEmailSender, EmailSender>();
Env.Load();
// Add services to the container
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Add Console logging
builder.Logging.AddDebug();

builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Look for the token in the cookie header
                    var token = context.Request.Cookies["jwt"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                }
            };
            options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

        };
        });
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseMiddleware<RateLimitingMiddleware>();

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
    */

}