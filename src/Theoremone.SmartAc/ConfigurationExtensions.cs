using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Theoremone.SmartAc.Infrastructure;
using Theoremone.SmartAc.Infrastructure.Persistence;

namespace Theoremone.SmartAc;

internal static class ConfigurationExtensions
{
    public static void AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();  
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SmartAC API",
                Description = "SmartAC Device Reporting API",
                Version = "v1"
            });

            c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "BearerAuth"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

  

    public static void UseOpenApiDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartAC API V1"); });
    }

    public static void MapSmartAcControllers(this WebApplication app)
    {
        app.MapControllers();
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    }

    public static void EnsureDatabaseSetup(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<SmartAcContext>();
        db.Database.EnsureCreated();
        SmartAcDataSeeder.Seed(db);
    }
}
