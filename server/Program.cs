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
using Microsoft.AspNetCore.RateLimiting;
public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        //inputsanitizer
        builder.Services.AddSingleton<OnlinePropertyBookingPlatform.Utility.InputSanitizer>();

        // Регистриране на services
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        }); ;

        // Регистрация на IEmailSender
        builder.Services.AddScoped<OnlinePropertyBookingPlatform.Utility.IEmailSender, EmailSender>();

        // CSRF Middleware
        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("http://localhost:5076")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        // Rate Limiting
        builder.Services.AddRateLimiter(options =>
        {
            // Логика при отхвърляне на заявка
            options.OnRejected = (context, _) =>
            {
                Console.WriteLine($"Rate limit exceeded for IP: {context.HttpContext.Connection.RemoteIpAddress}");
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers["Retry-After"] = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                    ? retryAfter.TotalSeconds.ToString()
                    : "60"; // 60 секунди като резервен вариант
                return new ValueTask();
            };

            // Глобален Fixed Window Rate Limiter
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var userIdentifier = context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString();
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: userIdentifier ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20, // Лимит на заявки
                        Window = TimeSpan.FromSeconds(5), // Интервал
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0 // Максимум 5 заявки в опашка
                    });
            });

            // Добавяне на Sliding Window Limiter
            options.AddPolicy("slidingPolicy", context =>
            {
                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: partitionKey => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 50, // Максимум заявки
                        Window = TimeSpan.FromSeconds(5), // В рамките на време
                        SegmentsPerWindow = 10, //  сегмента
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 20 // Максимум  заявки в опашката
                    });
            });
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

        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
        });
        });

        var app = builder.Build();


        // Use the CORS policy
        app.UseCors("AllowFrontend");

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
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();

        // Use Rate Limiter Middleware
        app.UseRateLimiter();
        app.MapControllers().RequireRateLimiting("slidingPolicy");

        // Map Controllers
        app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .RequireRateLimiting("slidingPolicy");



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