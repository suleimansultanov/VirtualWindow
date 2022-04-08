using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenUpdate;
using NasladdinPlace.Core.Utils.TasksAgent;
using System;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class PosTokenManagerExtensions
    {
        public static void AddPosTokenServices(this IServiceCollection services, IConfigurationReader configurationReader)
        {
            if (configurationReader == null)
                throw new ArgumentNullException(nameof(configurationReader));

            var encryptionKey = configurationReader.GetPosTokenServicesEncryptionKey();
            var tokenPrefix = configurationReader.GetPosTokenServicesTokenPrefix();
            var tokenValidityPeriod = configurationReader.GetPosTokenServicesTokenValidityPeriod();
            var adminPageBaseUrl = configurationReader.GetAdminPageBaseUrl();
            var tokenProviderCachePeriod = configurationReader.GetPosTokenServicesTokenProviderCachePeriod();

            var posTokenManagerOptions = new PosTokenServicesOptions
            {
                EncryptionKey = encryptionKey,
                TokenPrefix = ConfigurationReaderExt.CombineUrlParts( adminPageBaseUrl, tokenPrefix ),
                TokenValidityPeriod = tokenValidityPeriod,
                PosTokenProviderCachePeriod = tokenProviderCachePeriod
            };

            services.AddPosTokenServices(posTokenManagerOptions);
        }

        public static void AddPosTokenServices(this IServiceCollection services, PosTokenServicesOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            services.AddTransient<IPosTokenServicesFactory>(sp =>
            {
                var unitOfWorkFactory = sp.GetRequiredService<IUnitOfWorkFactory>();
                return new PosTokenServicesFactory(unitOfWorkFactory, options);
            });
            services.AddSingleton(sp =>
            {
                var tokenServicesFactory = sp.GetRequiredService<IPosTokenServicesFactory>();
                return tokenServicesFactory.CreatePosTokenProvider();
            });
            services.AddSingleton(sp =>
            {
                var tokenServicesFactory = sp.GetRequiredService<IPosTokenServicesFactory>();
                return tokenServicesFactory.CreatePosByTokenProvider();
            });
            services.AddSingleton(sp =>
            {
                var tasksAgent = sp.GetRequiredService<ITasksAgent>();
                var pointsOfSaleDisplaysTokenUpdater = sp.GetRequiredService<IPointsOfSaleDisplaysTokenUpdater>();
                return new PointsOfSaleTokenUpdateAgent(tasksAgent, pointsOfSaleDisplaysTokenUpdater);
            });
        }

        public static void UsePosTokenManager(this IApplicationBuilder app, IConfigurationReader configurationReader)
        {
            if (configurationReader == null)
                throw new ArgumentNullException(nameof(configurationReader));

            var tokenUpdatePeriod = configurationReader.GetPosTokenServicesTokenUpdatePeriod();

            var serviceProvider = app.ApplicationServices;
            var pointsOfSaleTokenUpdateAgent = serviceProvider.GetRequiredService<PointsOfSaleTokenUpdateAgent>();

            var agentOptions = TasksAgentOptions.FixedPeriodOfTime(tokenUpdatePeriod);
            pointsOfSaleTokenUpdateAgent.Start(agentOptions);
        }
    }
}