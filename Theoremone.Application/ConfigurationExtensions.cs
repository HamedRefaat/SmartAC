using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using Theoremone.SmartAc.Application.AlertsWrapper;
using Theoremone.SmartAc.Application.AlertsWrapper.Configrations;

namespace Theoremone.SmartAc.Application
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoppedServices();
            return services;
        }

        private static void AddScoppedServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(DeviceWrapper.DeviceWrapper));
            services.AddScoped(typeof(DeviceReadingAlertHandler));
            
        }


    }
}
