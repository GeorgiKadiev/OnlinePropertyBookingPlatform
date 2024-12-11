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
public class Program
{ 

public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            // Зареждане на appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //добавяне от Панчо

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

            // Регистрация на DbContext с MySQL
            builder.Services.AddDbContext<PropertyManagementContext>(options =>
            options.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 40))
            ));

            // Регистрация на CrudRepository
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
            // Добавяне на middleware за автентикация
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

                //подръжка за JWT
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(options =>
            {
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

                builder.Services.AddAuthorization();

            }

        }