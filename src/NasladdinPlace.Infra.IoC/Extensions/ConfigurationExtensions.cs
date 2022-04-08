using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Manager.Factory;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void AddConfigurationManager(this IServiceCollection services)
        {
            services.AddTransient(ConfigurationManagerFactory.Create);
        }
    }
}