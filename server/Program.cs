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
using System.Text; // Include the namespace for CrudRepository
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Antiforgery;

public class Program
{ 

public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
<<<<<<< HEAD
<<<<<<< HEAD
            // Зареждане на appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //добавяне от Панчо
=======
=======
>>>>>>> b20a9abec4863ff50fa01583b0ed660304cc5c0c
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

>>>>>>> 6fc7979225388f82ee54d92cca65241536245f26
            builder.Services.AddScoped<OnlinePropertyBookingPlatform.Utility.IEmailSender, EmailSender>();
    Env.Load();
    // Add services to the container
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole(); // Add Console logging
    builder.Logging.AddDebug();
    builder.Services.AddScoped<SecureRepository>();

        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<XssSanitizationFilter>();
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Rate Limiting 
        builder.Services.AddMemoryCache();
        builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
        builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        builder.Services.AddAntiforgery();


        // Регистриране на Rate Limiting middleware
        var app = builder.Build();
        app.UseIpRateLimiting();

        app.UseRouting();
        // CSRF защита за POST заявки
        app.Use((context, next) =>
        {
            if (string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
                antiforgery.ValidateRequestAsync(context);
            }
            return next(context);
        });
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseMiddleware<RateLimitingMiddleware>();

        app.UseHttpsRedirection();
        app.Run();


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
}