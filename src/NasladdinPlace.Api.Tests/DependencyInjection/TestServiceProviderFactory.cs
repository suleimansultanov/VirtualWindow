using CloudPaymentsClient.Domain.Factories.PaymentService;
using CloudPaymentsClient.Rest.Dtos.Payment;
using FirebaseCloudMessagingClient.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Services.Catalog;
using NasladdinPlace.Api.Services.Catalog.Contracts;
using NasladdinPlace.Api.Services.Checks;
using NasladdinPlace.Api.Services.Checks.Contracts;
using NasladdinPlace.Api.Services.MicroNutrients;
using NasladdinPlace.Api.Services.MicroNutrients.Contracts;
using NasladdinPlace.Api.Services.Pos.Display;
using NasladdinPlace.Api.Services.Pos.Display.Agents;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Services.Pos.Interactor;
using NasladdinPlace.Api.Services.Pos.RemoteController;
using NasladdinPlace.Api.Services.Spreadsheet.Creators;
using NasladdinPlace.Api.Services.Spreadsheet.Creators.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Factories;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Uploader;
using NasladdinPlace.Api.Services.Users.Account;
using NasladdinPlace.Api.Services.WebSocket.Controllers;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Factories;
using NasladdinPlace.Api.Services.WebSocket.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Mappers;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Api.Tests.Constants;
using NasladdinPlace.Api.Tests.Utils;
using NasladdinPlace.Api.Tests.Utils.CheckOnline;
using NasladdinPlace.Application.Services.Feedbacks;
using NasladdinPlace.Application.Services.Feedbacks.Contracts;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Factory;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers;
using NasladdinPlace.CheckOnline.Infrastructure;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.CheckOnline.Tools;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Services.ActivityManagement;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Core.Services.Check;
using NasladdinPlace.Core.Services.Check.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Makers;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Utilities.LabeledGoodsGrouper;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Utilities.LabeledGoodsGrouper.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Mappers;
using NasladdinPlace.Core.Services.Check.Detailed.Mappers.Contracts;
using NasladdinPlace.Core.Services.Check.Discounts.Managers;
using NasladdinPlace.Core.Services.Check.Extensions;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Mappers;
using NasladdinPlace.Core.Services.Check.Simple.Mappers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Check.Simple.Payment;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Helpers;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.CheckOnline.Helpers;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.DistancesToPointsOfSale;
using NasladdinPlace.Core.Services.Documents.Creators;
using NasladdinPlace.Core.Services.Documents.Creators.Conctracts;
using NasladdinPlace.Core.Services.Documents.Managers;
using NasladdinPlace.Core.Services.Documents.Managers.Contracts;
using NasladdinPlace.Core.Services.Documents.Validatiors;
using NasladdinPlace.Core.Services.Documents.Validatiors.Contracts;
using NasladdinPlace.Core.Services.Feedback;
using NasladdinPlace.Core.Services.Feedback.Builder.SenderInfo;
using NasladdinPlace.Core.Services.Feedback.Printer;
using NasladdinPlace.Core.Services.Formatters;
using NasladdinPlace.Core.Services.Formatters.Contracts;
using NasladdinPlace.Core.Services.HardToDetectLabels;
using NasladdinPlace.Core.Services.LabeledGoods.Partner;
using NasladdinPlace.Core.Services.LabeledGoods.Partner.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters.Contracts;
using NasladdinPlace.Core.Services.LabeledGoodsMerger;
using NasladdinPlace.Core.Services.LabelsDetectionStatistics;
using NasladdinPlace.Core.Services.LocationsDistance;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;
using NasladdinPlace.Core.Services.MicroNutrients.Calculator;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.OverdueGoods.Checker;
using NasladdinPlace.Core.Services.OverdueGoods.Makers;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Converter;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Mapper;
using NasladdinPlace.Core.Services.Payment.Adder;
using NasladdinPlace.Core.Services.Payment.Adder.Contracts;
using NasladdinPlace.Core.Services.Payment.Printer;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.PaymentCards;
using NasladdinPlace.Core.Services.PaymentCards.Contracts;
using NasladdinPlace.Core.Services.Pos.ContentSynchronization;
using NasladdinPlace.Core.Services.Pos.Display;
using NasladdinPlace.Core.Services.Pos.Doors;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Pos.LabeledGoodsCreator;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using NasladdinPlace.Core.Services.Pos.ScreenResolution;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Contracts;
using NasladdinPlace.Core.Services.Pos.Sensors.Checker;
using NasladdinPlace.Core.Services.Pos.Status;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.Core.Services.Pos.Version;
using NasladdinPlace.Core.Services.Pos.Version.Contracts;
using NasladdinPlace.Core.Services.Pos.WebSocket.Factory;
using NasladdinPlace.Core.Services.Printers.Factory;
using NasladdinPlace.Core.Services.Printers.Localization;
using NasladdinPlace.Core.Services.Purchase.Completion;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters.Contracts;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Factory;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker;
using NasladdinPlace.Core.Services.PurchasesResetter;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Core.Services.UnpaidPurchases.Finisher;
using NasladdinPlace.Core.Services.UserBalanceCalculator;
using NasladdinPlace.Core.Services.Users.Account;
using NasladdinPlace.Core.Services.Users.Manager;
using NasladdinPlace.Core.Services.Users.Test;
using NasladdinPlace.Core.Services.WebClient;
using NasladdinPlace.Core.Services.WebClient.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.Infra.IoC.Extensions;
using NasladdinPlace.Logging.PurchaseInitiating;
using NasladdinPlace.Logging.PurchaseInitiating.Contracts;
using NasladdinPlace.Spreadsheets.Services.Creators;
using NasladdinPlace.Spreadsheets.Services.Creators.Contracts;
using NasladdinPlace.Spreadsheets.Services.Credential;
using NasladdinPlace.Spreadsheets.Services.Credential.Contracts;
using NasladdinPlace.Spreadsheets.Services.Fetcher;
using NasladdinPlace.Spreadsheets.Services.Fetcher.Contracts;
using NasladdinPlace.Spreadsheets.Services.Formatters;
using NasladdinPlace.Spreadsheets.Services.Formatters.Contracts;
using NasladdinPlace.TestUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ILogger = Serilog.ILogger;

namespace NasladdinPlace.Api.Tests.DependencyInjection
{
    public static class TestServiceProviderFactory
    {
        private static readonly IApplicationDbContextFactory TestApplicationDbContextFactory =
            new TestApplicationDbContextFactory();

        private const int PositionConfigurationCacheDurabilityInMinutes = 1;
        private const int VatTaxCode = 3;

        private static readonly PosTokenServicesOptions PosTokenServicesOptions = new PosTokenServicesOptions
        {
            TokenPrefix = "https://online.nasladdin.club"
        };

        public static IServiceProvider Create()
        {
            return CreateServiceCollection().BuildServiceProvider();
        }

        public static ServiceCollection CreateServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddSingleton(sp => TestApplicationDbContextFactory);

            services.AddSingleton<INotificationsLogger, NotificationsLogger>();

            services.AddTransient<IJsonWebClient, JsonWebClient>();
            services.AddTransient<ISmsSender>(sp =>
            {
                var fakeSmsSender = new FakeAlwaysSuccessfulSmsSender(sp.GetRequiredService<INotificationsLogger>());
                fakeSmsSender.BalanceAlmostExceededHandler += (sender, balance) => { };
                fakeSmsSender.SmsServiceErrorHandler += (sender, smsLoggingInfo) => { };

                return fakeSmsSender;
            });

            services.AddTransient(sp =>
            {
                var mockTestUserChecker = new Mock<ITestUserInfoProvider>();
                mockTestUserChecker.Setup(c => c.IsTestUser(It.IsAny<ApplicationUser>())).Returns(false);
                mockTestUserChecker.Setup(c => c.IsTestUserAsync(It.IsAny<int>())).Returns(Task.FromResult(false));
                return mockTestUserChecker.Object;
            });

            services.AddScoped(sp =>
                sp.GetRequiredService<IApplicationDbContextFactory>().Create()
            );

            services.AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => { options.Password.RequireUppercase = false; });

            services.AddLogging();

            services.AddTransient(sp => new Mock<ILogger>().Object);

            services.AddTransient(sp => new Mock<Logging.ILogger>().Object);

            services.AddTransient(sp =>
            {
                var mockEnvironment = new Mock<IHostingEnvironment>();
                mockEnvironment
                    .Setup(m => m.EnvironmentName)
                    .Returns("Hosting:UnitTestEnvironment");
                return mockEnvironment.Object;
            });

            services.AddSingleton(sp =>
            {
                var mockConnectionDetector = new Mock<IPosConnectionStatusDetector>();
                mockConnectionDetector.Setup(d => d.Detect(It.IsAny<int>()))
                    .Returns(new PosConnectionInfo(PosConnectionStatus.Connected, new List<IPAddress>()));
                return mockConnectionDetector;
            });

            services.AddTransient<ICheckService, CheckService>();
            services.AddTransient(sp =>
                sp.GetRequiredService<Mock<IPosConnectionStatusDetector>>().Object
            );

            services.AddSingleton(sp =>
                new Mock<IPosRealTimeInfoDataStore>()
            );
            services.AddTransient(sp =>
                sp.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>().Object
            );

            services.AddTransient<INutrientsCalculator, NutrientsCalculator>();

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

            services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IFirstPayBonusAdder, FirstPayBonusAdder>();
            services.AddTransient<IPosOperationTransactionCheckItemsMaker, PosOperationTransactionCheckItemsMaker>();
            services.AddTransient<IOperationsManager, OperationsManager>();
            services.AddTransient<IPosOperationTransactionCreationUpdatingService, PosOperationTransactionCreationUpdatingService>();
            services.AddTransient<IOperationTransactionManager, OperationTransactionManager>();

            services.AddTransient<IPaymentCardsService, PaymentCardsService>();

            services.AddSingleton(sp =>
                new Mock<INasladdinWebSocketMessageSender>()
            );

            services.AddTransient<INutrientsService, NutrientsService>();

            services.AddTransient(sp =>
                sp.GetRequiredService<Mock<INasladdinWebSocketMessageSender>>().Object
            );

            services.AddTransient<IPosRemoteControllerFactory, PosRemoteControllerFactory>();

            services.AddTransient<IPosInteractor, PosInteractor>();

            services.AddTransient<ILocationsDistanceCalculator, HaversineLocationsDistanceCalculator>();

            services.AddTransient<IDistancesToPointsOfSaleCalculator, DistancesToPointsOfSaleCalculator>();

            services.AddTransient<IDetailedCheckGoodInstanceCreator, DetailedCheckGoodInstanceCreator>();
            services.AddTransient<IDetailedCheckMaker>(sp => new DetailedCheckMaker(
                    sp.GetRequiredService<IDetailedCheckGoodInstanceCreator>(),
                    "http://localhost:57603/api/fiscalizationInfos/{0}/qrCode",
                    "http://localhost:57604/fiscalizationInfos/{0}"
                )
            );
            services.AddTransient<ILabeledGoodsByGoodGrouper, LabeledGoodsByGoodGrouper>();
            services.AddTransient<ISimpleCheckMapper, SimpleCheckMapper>();
            services.AddTransient<ISimpleCheckMaker, SimpleCheckMaker>();
            services.AddTransient<ISenderInfoFactory, SenderInfoFactory>();
            services.AddTransient<IPaymentBalanceFactory, PaymentBalanceFactory>();
            services.AddTransient<IFeedbackService, FeedbackService>();
            services.AddTransient<IUserLatestOperationCheckMaker>(sp =>
                new UserLatestOperationCheckMaker(
                    sp.GetRequiredService<IUnitOfWorkFactory>(),
                    sp.GetRequiredService<ISimpleCheckMaker>()
                )
            );

            services.AddUsersUnpaidOperationsChecksMaker();

            services.AddTransient<IConfigurationReader, FakeConfigurationReader>();

            services.AddTransient<IUserPaymentBalanceCalculator, UserPaymentBalanceCalculator>();

            services.AddTransient<IOngoingPurchaseActivityManager>(sp =>
                new OngoingPurchaseActivityManager(
                    new ActivityManager<int>(), new ActivityManager<int>())
            );

            services.AddTransient(sp => sp.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork());

            services.AddTransient<IPosContentSynchronizer>(sp =>
                new PosContentSynchronizer(
                    new SimpleLabeledGoodsMerger(),
                    new UnsyncPosContentSeekerFactory(),
                    sp.GetRequiredService<Logging.ILogger>()
                )
            );

            services.AddTransient<IPosLabeledGoodsFromLabelsCreator, PosLabeledGoodsFromLabelsCreator>();

            services.AddTransient<IDocumentGoodsMovingCreator, DocumentGoodsMovingCreator>();
            services.AddTransient<IDocumentManager<DocumentGoodsMovingTableItem>, DocumentGoodsMovingManager>();
            services.AddTransient<IDocumentValidator<DocumentGoodsMovingTableItem>, DocumentValidator<DocumentGoodsMovingTableItem>>();

            services.AddTransient<ITasksAgent, TasksAgent>();

            services.AddTransient<IPosDisplaySettingsManager>(sp =>
                new PosDisplaySettingsManager(
                    new PosDisplaySettings(waitingSwitchingToDisconnectPageInSeconds: 30,
                        retryShowQrCodeAfterInSeconds: 10,
                        retryHideQrCodeAfterInSeconds: 10,
                        retryShowTimerAfterInSeconds: 10,
                        checkCommandsForRetrySendInSeconds: 10)
                ));

            services.AddSingleton<IPosDisplayManager, PosDisplayManager>();
            services.AddSingleton<IPosDoorsStateTracker, PosDoorsStateTracker>();
            services.AddSingleton<IPosTemperatureManager, PosTemperatureManager>();
            services.AddSingleton<IPosDisplayCommandsManager, PosDisplayCommandsManager>();
            services.AddSingleton<IPosDisplayCommandsQueueManager, PosDisplayCommandsQueueManager>();
            services.AddSingleton<IPosDisplayAgent, PosDisplayAgent>();
            services.AddTransient(sp => PosStatelessTokenManagerFactory.Create(PosTokenServicesOptions));
            services.AddPosTokenServices(PosTokenServicesOptions);
            services.AddTransient<IPosDisplayRemoteController>(sp =>
                new PosDisplayRemoteController(
                    sp.GetRequiredService<INasladdinWebSocketMessageSender>(),
                    sp.GetRequiredService<IPosDisplayManager>(),
                    sp.GetRequiredService<IPosTokenProvider>(),
                    sp.GetRequiredService<Logging.ILogger>()
                )
            );

            services.AddTransient<ILabelsDetectionStatisticsMaker, LabelsDetectionStatisticsMaker>();

            services.AddTransient<IPurchasesHistoryMaker, PurchasesHistoryMaker>();

            services.AddScoped<IFeedbackAppService, FeedbackAppService>();

            services.AddTransient<IPaymentDescriptionPrinter, PaymentRussianDescriptionPrinter>();
            services.AddTransient<IPaymentInfoCreator, PaymentInfoCreator>();
            services.AddTransient<ICheckPaymentService, CheckPaymentService>();
            services.AddSingleton<IPurchaseCompletionManager, PurchaseCompletionManager>();

            services.AddCheckManagerSmsSender(new CheckEditingNotificationMessages(
                refund: new NotificationMessage(
                    messageFormat:
                    "Чек №{0} от {1:d MMM} был проверен. Возврат {2:0} руб ожидайте в теч. 3 раб.дн. Остались вопросы - свяжитесь с нами +7 (495) 481-37-68",
                    isEnabled: true),
                additionOrVerification: new NotificationMessage(
                    messageFormat:
                    "Чек №{0} от {1:d MMM} был проверен. C карты дополнительно спишется {2:0} руб. Остались вопросы - свяжитесь с нами +7 (495) 481-37-68",
                    isEnabled: true)));

            services.AddCheckManager();
            services.AddTransient(sp =>
            {
                var serviceInfo = new ServiceInfo(
                    "pk_d60a2fee7b77994ebee662cb2b6a6",
                    "bffbabb12e1eb01eed2096ec429778ba"
                );
                return FakePaymentService.Create(new CloudPaymentsesServiceFactory(serviceInfo).CreatePaymentService());
            });

            services.AddSingleton(sp => new PurchaseManagerFactory(sp).Create());

            services.AddTransient<IUnpaidPurchaseFinisher>(sp => new UnpaidPurchaseFinisher(
                sp.GetRequiredService<IUnitOfWorkFactory>(),
                sp.GetRequiredService<IPurchaseManager>(),
                new Mock<Logging.ILogger>().Object));

            services.AddTransient<IUserPurchasesResetter, UnfinishedUserPurchasesResetter>();

            services.AddTransient<ILocalizedPrintersFactory<SimpleCheck>, SimpleCheckPrinterFactory>();

            services.AddSingleton(sp => new Mock<ITelegramChannelMessageSender>());

            services.AddTransient(sp => sp.GetRequiredService<Mock<ITelegramChannelMessageSender>>().Object);

            services.AddTransient(sp =>
            {
                var mockTelegramChannelsFactory = new Mock<ITelegramChannelsFactory>();
                mockTelegramChannelsFactory.Setup(f => f.Create(It.IsAny<TelegramChannel>()))
                    .Returns(sp.GetRequiredService<ITelegramChannelMessageSender>());
                return mockTelegramChannelsFactory.Object;
            });

            services.AddTransient(sp =>
            {
                var mockForm3DsHtmlMaker = new Mock<IForm3DsHtmlMaker>();
                mockForm3DsHtmlMaker
                    .Setup(m => m.Make(It.IsAny<Form3DsInfo>()))
                    .Returns(string.Empty);
                return mockForm3DsHtmlMaker.Object;
            });

            services.AddTransient<IHttpContextAccessor>(sp => new HttpContextAccessor
            {
                HttpContext = new TestHttpContext()
            });

            services.AddTransient<IPrintedCheckLinkFormatter>(sp =>
                new PrintedCheckLinkFormatter(string.Empty)
            );

            services.AddSingleton<ILinkTypeFormatProvider>(sp =>
                new LinkTypeFormatProvider(FeedbackRussianMessageContents.AdminPageLinkFakeFormat));
            services.AddSingleton<ILinkWrapper, TelegramLinkWrapper>();

            services.AddTransient<ILabeledGoodsPrinter, LabeledGoodsPrinter>();

            services.AddTransient<IHardToDetectLabelsManager, HardToDetectLabelsManager>();

            services.AddTransient<IHardToDetectLabelsPrinter, HardToDetectLabelsPrinter>();

            services.AddTransient<IPosLabeledGoodsBlocker, PosLabeledGoodsBlocker>();

            services.AddTransient<IPaymentCardConfirmationService, PaymentCardConfirmationService>();

            services.AddActionExecutionUtilities();

            services.AddPosOperationsAppService();

            services.AddTransient<PointsOfSaleController>();

            services.AddTransient<AccountingBalancesController>();

            services.AddTransient<PosController>();

            services.AddTransient<PosLogsController>();

            services.AddTransient<LabeledGoodsController>();

            services.AddTransient<LabeledGoodsPartnerController>();

            services.AddTransient<CurrentPurchaseController>();

            services.AddTransient<ChecksController>();

            services.AddTransient<PurchasesController>();

            services.AddTransient<PaymentCardsController>();

            services.AddTransient<CatalogController>();

            services.AddTransient<ICatalogService, CatalogService>();

            services.AddTransient<IHttpWebRequestProvider, HttpWebRequestProvider>();
            services.AddSingleton<IOnlineCashierAuth>(sp => new CheckOnlineAuth
            {
                ServiceUrl = CheckOnlineConfigurations.ServiceUrl,
                Login = CheckOnlineConfigurations.Login,
                Password = CheckOnlineConfigurations.Password,
                CertificateData = CheckOnlineConfigurations.CertificateData,
                CertificatePassword = CheckOnlineConfigurations.CertificatePassword
            });
            services.AddSingleton<IPosOperationTransactionTypeProvider, PosOperationTransactionTypeProvider>();
            services.AddSingleton<INotificationsLogger, NotificationsLogger>();
            services.AddFirebaseCloudMessagingClient(FirebaseConfigurations.ApiKey, new FakeConfigurationReader());

            services.AddTransient(sp => TestCheckOnlineRequestProviderFactory.Create(sp, true));
            services.AddTransient(sp =>
                CheckOnlineBuilderFactory.Create(sp.GetRequiredService<ICheckOnlineRequestProvider>()));
            services.AddTransient<ICheckOnlineManager>(sp => new CheckOnlineManager(
                sp.GetRequiredService<IUnitOfWorkFactory>(),
                sp.GetRequiredService<ICheckOnlineBuilder>(),
                sp.GetRequiredService<IOnlineCashierAuth>(),
                sp.GetRequiredService<IPosOperationTransactionTypeProvider>(),
                sp.GetService<NasladdinPlace.Logging.ILogger>(),
                taxCode: VatTaxCode //3: НДС 0%
            ));

            services.AddTransient<IPaymentDescriptionPrinter, PaymentRussianDescriptionPrinter>();

            services.AddSingleton<IConditionalPurchaseManager>(sp =>
                new ConditionalPurchaseManager(
                    sp.GetRequiredService<IUnitOfWorkFactory>(),
                    sp.GetRequiredService<ICheckManager>(),
                    TimeSpan.FromMinutes(20)));

            services.AddTransient<IDiscountsCheckManager, DiscountsCheckManager>();

            services.AddSingleton<IUserManager, FakeUserManager>();

            services.AddTransient<IHostingEnvironment, FakeProductionHostingEnvironment>();

            services.AddTransient<ITestUserInfoProvider, NoTestUserInfoProvider>();

            services.AddTransient<IAccountService, AccountService>();

            services.AddTransient<ILabeledGoodPartnerInfoService, LabeledGoodPartnerInfoService>();

            services.AddFiscalizationInfosQrCodeConversionServices(new FakeConfigurationReader());

            services.AddSingleton<IPosSensorControllerTypeConfigurationProvider>(sp =>
                new PosSensorControllerTypeConfigurationProvider(
                    positionConfigurationCacheDurabilityInMinutes: PositionConfigurationCacheDurabilityInMinutes));

            services.AddSingleton<IPosSensorsMeasurementsManagerFactory>(sp => new PosSensorsMeasurementsManagerFactory(
                sp.GetRequiredService<ITelegramChannelMessageSender>(),
                new PosSensorMeasurementsSettingsModel()
                {
                    LowerNormalAmperage = 1,
                    UpperNormalAmperage = 2,
                    FrontPanelPositionAbnormalPosition = FrontPanelPosition.Opened
                },
                sp.GetRequiredService<Logging.ILogger>()
            ));
            services.AddSingleton<IPosSensorControllerMeasurementsTracker, PosSensorControllerMeasurementsTracker>();

            services.AddTransient<ICheckCorrectnessStatusProcessor, CheckCorrectnessStatusProcessor>();

            services.AddTransient<AccountController>();

            services.AddTransient<PurchasesController>();

            services.AddSingleton<IGoogleCredential>(sp =>
                new GoogleServiceAccountCredential(new JsonCredentialParameters
                {
                    PrivateKey = GoogleServiceConfigurations.PrivateKey,
                    ClientEmail = GoogleServiceConfigurations.ClientEmail,
                    ClientId = GoogleServiceConfigurations.ClientId,
                    Type = GoogleServiceConfigurations.Type
                }, new[] { SheetsService.Scope.Spreadsheets }));

            services.AddSingleton(sp => new Mock<ISpreadsheetProvider>());

            services.AddSingleton<ISpreadsheetIdFetcher, SpreadsheetGoogleIdFromUrlFetcher>();
            services.AddSingleton<ISpreadsheetCellFormatter, SpreadsheetCellFormatter>();
            services.AddSingleton<ISpreadsheetDataRangeCreator, SpreadsheetDataRangeCreator>();

            services.AddSingleton(sp => sp.GetRequiredService<Mock<ISpreadsheetProvider>>().Object);

            services.AddSingleton<IFeedbackPrinter, FullFeedbackPrinter>();

            services.AddSingleton<IReportDataBatchProviderFactory, ReportDataBatchProviderFactory>();

            services.AddSingleton<ISpreadsheetsUploader>(sp => new SpreadsheetsUploader(
                sp.GetRequiredService<IUnitOfWorkFactory>(),
                sp.GetRequiredService<ISpreadsheetProvider>(),
                sp.GetRequiredService<IReportDataBatchProviderFactory>(),
                new Services.Spreadsheet.Models.SpreadsheetsUploadingTaskParameters(
                    retryDelay: TimeSpan.FromSeconds(5),
                    permittedRetryCount: 5,
                    reportDataExportingPeriodInDays: 100),
                sp.GetRequiredService<NasladdinPlace.Logging.ILogger>()));

            services.AddConfigurationManager();
            services.AddTransient<IPointsOfSaleVersionUpdateChecker, PointsOfSaleVersionUpdateChecker>();

            services.AddTransient<IPosScreenResolutionChecker, PosScreenResolutionChecker>();

            services.AddSingleton<IUntiedLabeledGoodsManager, UntiedLabeledGoodsManager>();

            services.AddSingleton<IReportFieldConvertsFactory, ReportFieldConvertsFactory>();
            services.AddSingleton<IPurchaseReportRecordFactory, PurchaseReportRecordFactory>();
            services.AddSingleton<IPurchaseReportRecordsCreator, PurchaseReportRecordsCreator>();

            services.AddSingleton<IGoodInstanceMapper, MoscowGoodInstanceMapper>();
            services.AddSingleton<IGoodInstancesByPosGrouper, GoodInstancesByPosGrouper>();
            services.AddSingleton<IOverdueGoodsInfoMaker, OverdueGoodsInfoMaker>();
            services.AddSingleton<IOverdueGoodsChecker, OverdueGoodsChecker>();

            services.AddSingleton<IUntiedLabeledGoodsInfoMessagePrinter, UntiedLabeledGoodsInfoRussianPrinter>();
            services.AddSingleton<IPrintedUntiedLabeledGoodsFormatter>(sp =>
                new PrintedUntiedLabeledGoodsFormatter("http://localhost:5001/PointsOfSale/{0}/LabeledGoods/Identification"));
            services.AddSingleton<IPrintDocumentGoodsMovingUntiedLabeledGoodsFormatter>(sp =>
                new PrintDocumentGoodsMovingUntiedLabeledGoodsFormatter("http://localhost:5001/GoodsMoving/DocumentGoodsMoving/{0}"));

            services.AddSingleton<IPurchaseInitiationLogger, PurchaseInitiationLogger>();

            services.AddTransient<IFiscalizationCheckPrinter, FiscalizationCheckPrinter>();

            services.AddTransient<WebSocketConnectionManager>();
            services.AddSingleton<IWebSocketConnectionManager, NasladdinWebSocketConnectionManager>();
            services.AddTransient<IWsControllerRouteCorrector, JoinGroupRouteCorrector>();
            services.AddTransient<IEventMessageToWsMessageMapper, EventMessageToWsMessageMapper>();
            services.AddTransient<IObjectDeserializer, ObjectDeserializer>();
            services.AddTransient<IWsControllerInvoker>(sp =>
            {
                var wsControllerRouteCorrector = sp.GetRequiredService<IWsControllerRouteCorrector>();
                var wsControllerInvoker = new WsControllerInvoker(sp.GetRequiredService<IServiceProvider>());
                return new WsControllerInvokerWithAdjustableRoutes(wsControllerInvoker, wsControllerRouteCorrector);
            });

            services.AddSingleton<IWsCommandsQueueProcessorFactory, WsCommandsQueueProcessorFactory>();


            return services;
        }

        public static void ExchangeService<TExchangableType>(ServiceCollection serviceCollection, Func<IServiceProvider, object> initReplacebleTypeFunc)
        {
            if (serviceCollection.Any(sc => sc.ServiceType == typeof(TExchangableType)))
            {
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var descriptor = serviceCollection.FirstOrDefault(sc => sc.ServiceType == typeof(TExchangableType));

                var index = serviceCollection.IndexOf(descriptor);

                serviceCollection.RemoveAt(index);

                var instance = initReplacebleTypeFunc(serviceProvider);

                var newDescriptor = new ServiceDescriptor(typeof(TExchangableType), instance);

                serviceCollection.Insert(index, newDescriptor);
            }
        }
    }
}