using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Services.Pos.ConnectionStatus;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Services.WebSocket.Controllers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Extensions;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories;
using NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;
using NasladdinPlace.Core.Services.Pos.Display;
using NasladdinPlace.Core.Services.Pos.Status;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.TestUtils;
using Serilog;
using System;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;

namespace NasladdinPlace.Api.Tests.Scenarios.WebSocketGroupConnection.DependencyInjection
{
    public class WebSocketServiceProviderFactory
    {
        public IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            
            services.AddWebSocketManager();
            
            services.AddSingleton(sp => new Mock<IPosDisplayRemoteController>());
            services.AddSingleton(sp => new Mock<IPosDisplayCommandsManager>());
            services.AddSingleton(sp => new Mock<IPosDoorsStateTracker>());

            services.AddTransient<IIdFromGroupFetcher, IdFromGroupFetcher>();

            services.AddTransient(sp => new Mock<ILogger>().Object);

            services.AddTransient(sp => new Mock<Logging.ILogger>().Object);

            services.AddTransient(sp =>
            {
                var mockPosDisplayInteractor = sp.GetRequiredService<Mock<IPosDisplayRemoteController>>();
                return mockPosDisplayInteractor.Object;
            });

            services.AddTransient(sp =>
            {
                var mockPosDisplayCommandsManager = sp.GetRequiredService<Mock<IPosDisplayCommandsManager>>();
                return mockPosDisplayCommandsManager.Object;
            });

            services.AddTransient(sp =>
            {
                var mockPosDoorsStateManager = sp.GetRequiredService<Mock<IPosDoorsStateTracker>>();
                return mockPosDoorsStateManager.Object;
            });
            
            services.AddTransient<GroupsController>();
            services.AddTransient<IPosConnectionStatusDetector, PosConnectionStatusDetector>();

            services.AddSingleton(sp =>
                new Mock<IPosRealTimeInfoDataStore>()
            );
            services.AddTransient(sp =>
                sp.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>().Object
            );

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

            services.AddTransient<IPosConnectionStatusNotifications, PosConnectionStatusNotifications>();
            services.AddTransient(sp => new Mock<ITelegramChannelMessageSender>().Object);

            services.AddTransient(sp =>
            {
                var mockTelegramChannelsFactory = new Mock<ITelegramChannelsFactory>();
                mockTelegramChannelsFactory.Setup(f => f.Create(It.IsAny<TelegramChannel>()))
                    .Returns(sp.GetRequiredService<ITelegramChannelMessageSender>());
                return mockTelegramChannelsFactory.Object;
            });

            services.AddSingleton<IWebSocketGroupHandlerFactory>(sp =>
                new WebSocketGroupHandlerFactory(sp.GetRequiredService<IServiceProvider>()));

            return services.BuildServiceProvider();
        }
    }
}