using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.CloudKassir;
using NasladdinPlace.Core.Services.CloudKassir.CloudKassirAgent;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Fiscalization.Services;
using NasladdinPlace.Logging;
using System;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;

namespace NasladdinPlace.Api.Services.CloudKassir
{
    public static class CloudKassirExtensions
    {
        public static void AddCloudKassirIntegration(this IServiceCollection services, IConfigurationReader configurationReader)
        {
            services.AddTransient<ICloudKassirManager>(sp => new CloudKassirManager(
                    sp.GetRequiredService<IUnitOfWorkFactory>(),
                    sp.GetRequiredService<ICloudKassirService>(),
                    sp.GetService<ILogger>(),
                    configurationReader.GetCloudKassirInn(),
                    (TaxationSystem) configurationReader.GetCloudKassirTaxationSystem()
                ));
        }

        //TODO enable this in Startup
        public static void UseCloudKassirAgent(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var cloudKassirAgent = serviceProvider.GetRequiredService<ICloudKassirAgent>();
            var cloudKassirManager = serviceProvider.GetRequiredService<ICloudKassirManager>();

            cloudKassirAgent.OnFoundPendingFiscalization += (sender, fiscalizationInfoIds) =>
            {
                Task.Run(() => cloudKassirManager.MakeReFiscalizationAsync(fiscalizationInfoIds));
            };
            //TODO get value from configuration
            cloudKassirAgent.Start(TimeSpan.FromMinutes(10));
        }
    }
}
