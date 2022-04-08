using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Manager.Contracts;
using NasladdinPlace.Core.Services.Configuration.Validators.Factory;

namespace NasladdinPlace.Core.Services.Configuration.Manager.Factory
{
    public static class ConfigurationManagerFactory
    {
        public static IConfigurationManager Create(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            var valueValidatorsFactory = new ConfigurationValueValidatorsFactory();
            return new ConfigurationManager(unitOfWorkFactory, valueValidatorsFactory);
        }
    }
}