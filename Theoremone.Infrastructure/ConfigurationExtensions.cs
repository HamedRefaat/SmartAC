using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Theoremone.SmartAc.Application.Common.Interfaces;
using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Infrastructure.Identity;
using Theoremone.SmartAc.Infrastructure.Persistence;
using Theoremone.SmartAc.Infrastructure.Persistence.Repositories;
using Theoremone.SmartAc.Infrastructure.Services;

namespace Theoremone.SmartAc.Infrastructure;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSQLiteContext(configuration);
        services.AddTransientServices();
        services.AddScopedServices();
        services.AddJwtAuthentication(configuration);
        return services;
    }

   

    private static void AddSQLiteContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("smartac") ?? "Data Source=SmartAc.db";

        services.AddSqlite<SmartAcContext>(connectionString);
    }

    private static void AddTransientServices(this IServiceCollection services)
    {
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<SmartAcJwtService>();
        services.AddTransient<IAuthorizationHandler, ValidTokenAuthorizationHandler>();
    }

    private static void AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IDeviceRepo, DeviceRepo>();
        services.AddScoped<IDeviceRegistrationRepo, DeviceRegistrationRepo>();
        services.AddScoped<IDeviceReadingRepo, DeviceReadingRepo>();
        services.AddScoped<IAlertRepo, AlertRepo>();
    }

    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var signingKey = configuration["Jwt:Key"];

        services.AddAuthorization();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidIssuer = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("DeviceAdmin", policy =>
                policy.RequireRole(SmartAcJwtService.JwtScopeDeviceAdminService)
            );

            options.AddPolicy("DeviceIngestion", policy =>
            {
                policy.RequireRole(SmartAcJwtService.JwtScopeDeviceIngestionService);
                policy.AddRequirements(new ValidTokenRequirement());
            });
        });

        services.AddHttpContextAccessor();
    }
}
