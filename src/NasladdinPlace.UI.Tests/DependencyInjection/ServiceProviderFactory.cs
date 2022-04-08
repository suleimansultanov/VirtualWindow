using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Status;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.TestUtils;
using NasladdinPlace.UI.Controllers;
using NasladdinPlace.UI.Services.NasladdinApi;
using System;
using System.Collections.Generic;
using System.Net;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Core.Services.Authorization;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Disabled;
using NasladdinPlace.Core.Services.Pos.Groups.Groupers.LabeledGoods;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.WebSocket.Factory;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Services.Authorization;
using NasladdinPlace.UI.Services.AuthTokenManagement;
using NasladdinPlace.UI.Services.Users;
using ILogger = Serilog.ILogger;
using NasladdinPlace.Infra.IoC.Extensions;
using MakersController = NasladdinPlace.UI.Controllers.Api.MakersController;

namespace NasladdinPlace.UI.Tests.DependencyInjection
{
    public class ServiceProviderFactory
    {
        private static readonly PosTokenServicesOptions PosTokenServicesOptions = new PosTokenServicesOptions
        {
            TokenPrefix = "https://online.nasladdin.club"
        };

        public IServiceProvider CreateServiceProvider(ApplicationDbContext context)
        {
            var services = new ServiceCollection();

            services.AddTransient(sp => context);
            
            services.AddScoped(sp =>
                new TestApplicationDbContextFactory().Create()
            );
            
            services.AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddTransient(sp => PosStatelessTokenManagerFactory.Create(PosTokenServicesOptions));
            services.AddPosTokenServices(PosTokenServicesOptions);

            services.AddSingleton(sp =>
            {
                var mockConnectionDetector = new Mock<IPosConnectionStatusDetector>();
                mockConnectionDetector.Setup(d => d.Detect(It.IsAny<int>())).Returns(new PosConnectionInfo(PosConnectionStatus.Connected, new List<IPAddress>()));
                return mockConnectionDetector;
            });

            services.AddTransient(sp =>
                sp.GetRequiredService<Mock<IPosConnectionStatusDetector>>().Object
            );

            services.AddTransient(sp => new Mock<ILogger>().Object);
            services.AddTransient(sp => new Mock<Logging.ILogger>().Object);

            services.AddTransient<IPosRealTimeInfoDataStore, PosRealTimeInfoDataStore>();

            services.AddSingleton<IPosStateSettingsProvider>(sp => new PosStateSettingsProvider(
                new PosStateSetting(
                    new PosTemperatureMeasurementsSettings(
                        lowerNormalTemperature: 0,
                        upperNormalTemperature: 6,
                        calcAverageTemperatureEveryInMinutes: 35,
                        noTemperatureUpdatesTimeoutInMinutes: 10,
                        abnormalTemperatureAlertMutingPeriodAfterAdminPosOperationInMinutes: 15
                    ),
                    new PosStateHistoricalDataSettings(
                        posStateDataLifeTimePeriodInDays: 10,
                        deletingObsoleteHistoricalDataStartTime: TimeSpan.Parse("01:00:00")
                    ),
                    new PosStateChartSettings(
                        measurementDefaultPeriodInMinutes: 30,
                        chartRefreshFrequencyInSeconds: 60,
                        chartDateTimeDisplayFormat: "dd.MM.yyyy HH:mm"
                    )
                )));

            services.AddTransient<IUnitOfWorkFactory>(sp =>
                new UnitOfWorkFactory(
                    new TestApplicationDbContextFactory(),
                    sp.GetRequiredService<IPosRealTimeInfoDataStore>()
                )
            );

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddMvc();
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.Expiration = TimeSpan.FromMinutes(120);
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Login");
                });
            
            services.AddLogging();

            services.AddSingleton<IAuthTokenManager, AuthTokenManager>();

            services.AddSingleton(sp => new Mock<INasladdinApiClient>());

            services.AddTransient(sp =>
            {
                var mockNasladdinApiClient = sp.GetRequiredService<Mock<INasladdinApiClient>>();
                return mockNasladdinApiClient.Object;
            });
            
            services.AddTransient<AccountController>();
            services.AddTransient<UI.Controllers.Api.LogsController>();
            services.AddTransient<ILabeledGoodsByPosGrouper, LabeledGoodsByPosGrouper>();
            services.AddTransient<IDisabledLabeledGoodsManager, DisabledLabeledGoodsManager>();
            services.AddTransient<UI.Controllers.Api.LabeledGoodsController>();
            services.AddTransient<PointsOfSaleController>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserAppFeaturesAccessChecker, UserAppFeaturesAccessChecker>();
            services.AddTransient<IUserAccessGroupMembershipManager, UserAccessGroupMembershipManager>();
            services.AddTransient<MobileAppsStoreRedirection>();
            services.AddAppFeatures();
            services.AddTransient<MakersController>();
            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = services.BuildServiceProvider()
                    }
                });
            services.AddSingleton<IWsCommandsQueueProcessorFactory, WsCommandsQueueProcessorFactory>();

            return services.BuildServiceProvider();
        }
    }
}