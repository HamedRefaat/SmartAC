using System.Text.Json.Serialization;
using Theoremone.SmartAc;
using Theoremone.SmartAc.Api.Filters;
using Theoremone.SmartAc.Application;
using Theoremone.SmartAc.Application.AlertsWrapper.Configrations;
using Theoremone.SmartAc.Application.Common.Exceptions;
using Theoremone.SmartAc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        
    });
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddOpenApiDocumentation();

builder.Services.Configure<AlertsConfigrations>(
    builder.Configuration.GetSection(AlertsConfigrations.Section));

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.EnsureDatabaseSetup();
}

app.UseHttpsRedirection();

app.UseOpenApiDocumentation();
app.UseAuthentication();
app.UseAuthorization();

app.MapSmartAcControllers();
app.Run();