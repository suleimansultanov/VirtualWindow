using AspNet.Security.OAuth.Validation;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using CloudPaymentsClient.Domain.Factories.PaymentService;
using CloudPaymentsClient.Rest.Dtos.Payment;
using FirebaseCloudMessagingClient.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.MvcFilters;
using NasladdinPlace.Api.Services.ActivityManagement.OngoingPurchase.Extensions;
using NasladdinPlace.Api.Services.ActivityManagement.PosComponents.Extensions;
using NasladdinPlace.Api.Services.Authentication;
using NasladdinPlace.Api.Services.Authentication.Contracts;
using NasladdinPlace.Api.Services.BankingCardConfirmation.Extensions;
using NasladdinPlace.Api.Services.Catalog;
using NasladdinPlace.Api.Services.Catalog.Contracts;
using NasladdinPlace.Api.Services.CheckOnline;
using NasladdinPlace.Api.Services.Checks;
using NasladdinPlace.Api.Services.Checks.Contracts;
using NasladdinPlace.Api.Services.Diagnostics.Extensions;
using NasladdinPlace.Api.Services.EmailSender.Extensions;
using NasladdinPlace.Api.Services.EmailSender.Models;
using NasladdinPlace.Api.Services.ExceptionHandling;
using NasladdinPlace.Api.Services.Feedback;
using NasladdinPlace.Api.Services.FileStorage;
using NasladdinPlace.Api.Services.Identity.Extensions;
using NasladdinPlace.Api.Services.Logs.User.Extensions;
using NasladdinPlace.Api.Services.Logs.Writers;
using NasladdinPlace.Api.Services.MicroNutrients;
using NasladdinPlace.Api.Services.MicroNutrients.Contracts;
using NasladdinPlace.Api.Services.OneCSync;
using NasladdinPlace.Api.Services.OngoingPurchase;
using NasladdinPlace.Api.Services.OverdueGoods;
using NasladdinPlace.Api.Services.Pos.ConnectionStatus;
using NasladdinPlace.Api.Services.Pos.Display;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Services.Pos.Interactor;
using NasladdinPlace.Api.Services.Pos.Interactor.Factory;
using NasladdinPlace.Api.Services.Pos.PosDiagnostics.Extensions;
using NasladdinPlace.Api.Services.Pos.PosLogs;
using NasladdinPlace.Api.Services.Pos.RemoteController;
using NasladdinPlace.Api.Services.Pos.Temperature.Extensions;
using NasladdinPlace.Api.Services.PosNotifications.Extensions;
using NasladdinPlace.Api.Services.PosScreenResolution.Extensions;
using NasladdinPlace.Api.Services.PromotionNotifications;
using NasladdinPlace.Api.Services.PurchaseManager;
using NasladdinPlace.Api.Services.PurchaseManager.Conditional;
using NasladdinPlace.Api.Services.QrCodeGeneration;
using NasladdinPlace.Api.Services.Reports;
using NasladdinPlace.Api.Services.Spreadsheet.Extensions;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
using NasladdinPlace.Api.Services.TestUser.Extensions;
using NasladdinPlace.Api.Services.UntiedLabeledGoods;
using NasladdinPlace.Api.Services.Users;
using NasladdinPlace.Api.Services.Users.Account;
using NasladdinPlace.Api.Services.UsersBonusPoints;
using NasladdinPlace.Api.Services.WebSocket.Managers.Extensions;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Extensions;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Extensions;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Services.Check;
using NasladdinPlace.Core.Services.Check.Contracts;
using NasladdinPlace.Core.Services.Check.Discounts.Managers;
using NasladdinPlace.Core.Services.Check.Extensions;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Configuration.Helpers;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.DistancesToPointsOfSale;
using NasladdinPlace.Core.Services.Documents.Creators;
using NasladdinPlace.Core.Services.Documents.Creators.Conctracts;
using NasladdinPlace.Core.Services.Documents.Managers;
using NasladdinPlace.Core.Services.Documents.Managers.Contracts;
using NasladdinPlace.Core.Services.Documents.Validatiors;
using NasladdinPlace.Core.Services.Documents.Validatiors.Contracts;
using NasladdinPlace.Core.Services.Formatters;
using NasladdinPlace.Core.Services.Formatters.Contracts;
using NasladdinPlace.Core.Services.HardToDetectLabels;
using NasladdinPlace.Core.Services.LabeledGoods.Partner;
using NasladdinPlace.Core.Services.LabeledGoods.Partner.Contracts;
using NasladdinPlace.Core.Services.LabeledGoodsMerger;
using NasladdinPlace.Core.Services.LabeledGoodsMerger.Contracts;
using NasladdinPlace.Core.Services.LabelsDetectionStatistics;
using NasladdinPlace.Core.Services.LocationsDistance;
using NasladdinPlace.Core.Services.MessageSender.Sms.Extensions;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.MicroNutrients.Calculator;
using NasladdinPlace.Core.Services.NotificationsCenter;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.Payment.Adder;
using NasladdinPlace.Core.Services.Payment.Adder.Contracts;
using NasladdinPlace.Core.Services.Payment.Printer;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.PaymentCards;
using NasladdinPlace.Core.Services.PaymentCards.Contracts;
using NasladdinPlace.Core.Services.Pos.ContentSynchronization;
using NasladdinPlace.Core.Services.Pos.Display;
using NasladdinPlace.Core.Services.Pos.Extensions;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Pos.LabeledGoodsCreator;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenUpdate;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using NasladdinPlace.Core.Services.Pos.Sensors.Checker;
using NasladdinPlace.Core.Services.Pos.Status;
using NasladdinPlace.Core.Services.PosDiagnostics;
using NasladdinPlace.Core.Services.PosDiagnostics.Contracts;
using NasladdinPlace.Core.Services.Printers.Factory;
using NasladdinPlace.Core.Services.Printers.Localization;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker;
using NasladdinPlace.Core.Services.PurchasesResetter;
using NasladdinPlace.Core.Services.TextFileReader;
using NasladdinPlace.Core.Services.UnpaidPurchases.Finisher;
using NasladdinPlace.Core.Services.UnpaidPurchases.Monitor;
using NasladdinPlace.Core.Services.UserBalanceCalculator;
using NasladdinPlace.Core.Services.Users.Account;
using NasladdinPlace.Core.Services.Users.Manager;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.DAL.EntityConfigurations.Contracts;
using NasladdinPlace.DAL.EntityConfigurations.Default;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.Infra.IoC.Extensions;
using NasladdinPlace.Infra.IoC.Factories.Configuration;
using NasladdinPlace.Infra.IoC.Factories.DbContext;
using NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission;
using NasladdinPlace.Logging.PurchaseInitiating;
using NasladdinPlace.Logging.PurchaseInitiating.Contracts;
using NasladdinPlace.TelegramBot;
using NasladdinPlace.TelegramBot.Extentions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.CloudKassir;
using ConnectionInfo = NasladdinPlace.Api.Services.EmailSender.Models.ConnectionInfo;
using ILogger = Serilog.ILogger;

namespace NasladdinPlace.Api
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }
        private IApplicationDbContextFactory ApplicationDbContextFactory { get; }
        private IConfigurationReader ConfigurationReader { get; }
        private bool IsDevelopment { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddJsonFile("azurekeyvault.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            IsDevelopment = env.IsDevelopment();

            if (IsDevelopment)
            {
                builder.AddUserSecrets<Startup>();
            }
            else
            {
                builder.AddAzureKeyVault(
                    vault: $"https://{Configuration["AzureKeyVault:Vault"]}.vault.azure.net/",
                    clientId: Configuration["AzureKeyVault:ClientId"],
                    clientSecret: Configuration["AzureKeyVault:ClientSecret"]
                );
            }

            Configuration = builder.Build();

            ApplicationDbContextFactory = ApplicationDbContextFactoryFactory.Create(
                Configuration.GetConnectionString("DefaultConnection")
            );
            ConfigurationReader = ConfigurationReaderFactory.Create(ApplicationDbContextFactory);

            ApplyMigrations();

            ConfigurationReader.LoadConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(sp => ConfigurationReader);

            services.AddScoped<IEntityConfigurations>(sp => new EntityConfigurations());

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.Setup(Configuration.GetConnectionString("DefaultConnection"), "NasladdinPlace.DAL");
                options.UseOpenIddict<int>();
            });

            services.AddIdentity<ApplicationUser, Role>(options =>
                {
                    options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                    options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                    options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddChangePhoneNumberTotpTokenProvider();

            services.AddScoped<IUserManager, IdentityUserManagerWrapper>();

            services.AddTransient<IAuthTicketFactory, AuthTicketFactory>();

            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>()
                        .ReplaceDefaultEntities<int>();
                })

                .AddServer(options =>
                {
                    options.DisableScopeValidation();
                    options.UseMvc();
                    options.AcceptAnonymousClients();
                    options.EnableTokenEndpoint($"/{Routes.TokenEndpoint}");
                    options.AllowPasswordFlow();
                    options.AllowRefreshTokenFlow();
                    options.SetRefreshTokenLifetime(TimeSpan.Parse(Configuration["Auth:RefreshTokenLifetime"]));
                    options.SetAccessTokenLifetime(TimeSpan.Parse(Configuration["Auth:AccessTokenLifetime"]));

                    if (bool.Parse(Configuration["Auth:DisableHttpsRequirement"]))
                    {
                        options.DisableHttpsRequirement();
                    }
                })
                .AddValidation();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                        return Task.FromResult(0);
                    }
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OAuthValidationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OAuthValidationDefaults.AuthenticationScheme;
            });

            services.AddMvc(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                    config.Filters.Add(new ModelStateValidationFilterAttribute());
                    config.Filters.Add(new RequestExecutionDelayFilterAttribute());
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            services.AddTransient(sp =>
                new LogWriterViaWebSocket(sp.GetRequiredService<NasladdinWebSocketDuplexEventMessageHandler>()));

            services.AddSerilog(sp => sp.GetRequiredService<LogWriterViaWebSocket>());

            services.AddSingleton<IEntityConfigurationsFactory, EntityConfigurationsFactory>();
            services.AddSingleton(sp => ApplicationDbContextFactory);
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.AddSingleton<IPointsOfSaleDisplaysTokenUpdater, PointsOfSaleDisplaysTokenUpdater>();
            services.AddPosTokenServices(ConfigurationReader);
            services.AddSingleton<ITasksAgent, TasksAgent>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IObjectDeserializer, ObjectDeserializer>();
            services.AddTransient<IIdFromGroupFetcher, IdFromGroupFetcher>();

            services.AddWebSocketManager();

            services.AddFileSystemStorage();

            services.AddSingleton(sp => Configuration);

            services.AddSingleton<INotificationsLogger, NotificationsLogger>();
            services.AddSmsMessageSender(ConfigurationReader,
                ServerModeHelper.IsServerModeDevelopment(ConfigurationReader));
            services.AddScoped<ILabeledGoodsMerger, SimpleLabeledGoodsMerger>();
            services.AddScoped(sp => new UnsyncPosContentSeekerFactory());
            services.AddScoped<IPosContentSynchronizer, PosContentSynchronizer>();

            services.AddDetailedCheck(ConfigurationReader);

            services.AddSimpleCheck();

            services.AddUsersUnpaidOperationsChecksMaker();

            services.AddTransient<IFirstPayBonusAdder, FirstPayBonusAdder>();
            services.AddTransient<IUserPaymentBalanceCalculator, UserPaymentBalanceCalculator>();
            services.AddTransient<IPaymentBalanceFactory, PaymentBalanceFactory>();
            services.AddTransient<IPosOperationTransactionCheckItemsMaker, PosOperationTransactionCheckItemsMaker>();
            services.AddTransient<IOperationTransactionManager, OperationTransactionManager>();
            services.AddTransient<IPosOperationTransactionCreationUpdatingService, PosOperationTransactionCreationUpdatingService>();
            services.AddTransient<IOperationsManager, OperationsManager>();
            services.AddTransient<IPosRemoteControllerFactory, PosRemoteControllerFactory>();
            services.AddTransient<IPosInteractor, PosInteractor>();
            services.AddSingleton<IPosInteractorFactory, PosInteractorFactory>();
            services.AddTransient<ILocationsDistanceCalculator, HaversineLocationsDistanceCalculator>();
            services.AddSingleton<IDistancesToPointsOfSaleCalculator, DistancesToPointsOfSaleCalculator>();

            services.AddTransient<ITextFileReader>(sp =>
                new TextFileReader(
                    sp.GetRequiredService<IHostingEnvironment>().WebRootPath
                )
            );
            services.AddTransient<IPosLabeledGoodsFromLabelsCreator, PosLabeledGoodsFromLabelsCreator>();
            services.AddTransient<IPosConnectionStatusDetector, PosConnectionStatusDetector>();
            services.AddSingleton<IPosRealTimeInfoDataStore, PosRealTimeInfoDataStore>();

            services.AddTransient<IDocumentGoodsMovingCreator, DocumentGoodsMovingCreator>();
            services.AddTransient<IDocumentManager<DocumentGoodsMovingTableItem>, DocumentGoodsMovingManager>();
            services.AddTransient<IDocumentValidator<DocumentGoodsMovingTableItem>, DocumentValidator<DocumentGoodsMovingTableItem>>();

            services.AddTransient<INutrientsService, NutrientsService>();

            services.AddTransient<IPaymentCardsService, PaymentCardsService>();

            services.AddTransient<IPosDisplaySettingsManager>(sp => new PosDisplaySettingsManager(
                new PosDisplaySettings(
                        int.Parse(Configuration["PosDisplaySettings:WaitingSwitchingToDisconnectPageInSeconds"]),
                        int.Parse(Configuration["PosDisplaySettings:RetryShowQrCodeAfterInSeconds"]),
                        int.Parse(Configuration["PosDisplaySettings:RetryHideQrCodeAfterInSeconds"]),
                        int.Parse(Configuration["PosDisplaySettings:RetryShowTimerAfterInSeconds"]),
                        int.Parse(Configuration["PosDisplaySettings:CheckCommandsForRetrySendInSeconds"])
                    )));
            services.AddPosDisplayServices();

            services.AddTransient<IPosDisplayRemoteController>(sp => new PosDisplayRemoteController(
                sp.GetRequiredService<NasladdinWebSocketDuplexEventMessageHandler>(),
                sp.GetRequiredService<IPosDisplayManager>(),
                sp.GetRequiredService<IPosTokenProvider>(),
                sp.GetRequiredService<Logging.ILogger>()
            ));

            services.AddTransient<IPurchasesHistoryMaker, PurchasesHistoryMaker>();
            services.AddTransient<IUserPurchasesResetter, UnfinishedUserPurchasesResetter>();
            services.AddSingleton<ILabeledGoodsPrinter, LabeledGoodsPrinter>();
            services.AddSingleton<ILabeledGoodPartnerInfoService, LabeledGoodPartnerInfoService>();
            services.AddTransient<ILabelsDetectionStatisticsMaker, LabelsDetectionStatisticsMaker>();
            services.AddTransient<IHardToDetectLabelsPrinter, HardToDetectLabelsPrinter>();
            services.AddTransient<IHardToDetectLabelsManager, HardToDetectLabelsManager>();

            services.AddTransient<ICatalogService, CatalogService>();

            services.AddTransient<ICloudPaymentsServiceFactory>(sp => new CloudPaymentsesServiceFactory(
                new ServiceInfo(
                        id: Configuration["CloudPayments:ServiceId"],
                        secret: Configuration["CloudPayments:Secret"]
                    )));

            services.AddSingleton(sp => sp.GetRequiredService<ICloudPaymentsServiceFactory>().CreatePaymentService());

            services.AddSingleton(sp => sp.GetRequiredService<ICloudPaymentsServiceFactory>().CreateCloudKassirService());

            services.AddCloudKassirIntegration(ConfigurationReader);

            services.AddTelegramBot(
                apiKey: Configuration["Telegram:Bot:ApiKey"],
                telegramChannelId: ServerModeHelper.GetTelegramChannelId(ConfigurationReader, Configuration));

            services.AddSingleton<IMessagePrintersFactory, MessagePrintersFactory>();
            services.AddSingleton<INotificationsCenter>(sp => new NotificationsCenter(new[]
                {
                    new TelegramNotificationChannel(sp.GetRequiredService<ITelegramChannelMessageSender>()),
                },
                sp.GetRequiredService<IMessagePrintersFactory>())
            );

            services.AddEmailSender(new SmtpServerInfo(
                new ConnectionInfo(
                    url: Configuration["SmtpServerInfo:ConnectionInfo:Url"],
                    port: int.Parse(Configuration["SmtpServerInfo:ConnectionInfo:Port"])),
                new MailboxCredentials(
                    email: Configuration["SmtpServerInfo:MailboxCredentials:Email"],
                    password: Configuration["SmtpServerInfo:MailboxCredentials:Password"]))
            );
            services.AddFeedbackService();
            services.AddOverdueGoodsChecker(ConfigurationReader);
            services.AddTransient<INutrientsCalculator, NutrientsCalculator>();
            services.AddSingleton<ILinkTypeFormatProvider>(sp =>
                new LinkTypeFormatProvider(ConfigurationReader.GetAdminPageLinkFormats()));
            services.AddSingleton<ILinkWrapper, TelegramLinkWrapper>();

            services.AddOngoingPurchaseActivityMonitor(ConfigurationReader);

            services.AddSingleton<ILocalizedPrintersFactory<SimpleCheck>, SimpleCheckPrinterFactory>();

            services.AddPurchaseManager(detailedCheckAdminPageBaseUrl:
                ConfigurationReaderExt.CombineUrlParts(ConfigurationReader.GetAdminPageBaseUrl(), ConfigurationReader.GetDetailedCheckAdminPageUrl()));

            services.AddSingleton<IUnpaidPurchaseFinisher, UnpaidPurchaseFinisher>();
            services.AddSingleton<IUnpaidPurchasesMonitor>(sp =>
                new UnpaidPurchasesMonitor(
                    sp.GetRequiredService<ITasksAgent>(),
                    sp.GetRequiredService<IUnpaidPurchaseFinisher>(),
                    delaySinceTheBeginningOfADay: TimeSpan.FromHours(10),
                    delayBetweenTaskExecutions: TimeSpan.FromDays(1),
                    considerUnpaidAfter: TimeSpan.FromMinutes(30)
                )
            );

            services.AddBankingCardConfirmationService(
                ConfigurationReaderExt.CombineUrlParts(ConfigurationReader.GetJwtBearerOptionsAudience(), ConfigurationReader.GetPayment3DsResultUrlsCompletion())
            );

            services.AddFirebaseCloudMessagingClient(apiKey: Configuration["Firebase:ApiKey"], configurationReader: ConfigurationReader);

            services.AddTestUserInfoProvider(ConfigurationReader);

            services.AddActionExecutionUtilities();

            services.AddTransient<IPosDiagnosticsFactory, PosDiagnosticsFactory>();

            services.AddPromotionNotifications();

            services.AddReports(ConfigurationReader);

            services.AddPosLabeledGoodsBlocker();

            services.AddScheduledSpreadsheetsUploader(ConfigurationReader,
                new GoogleServiceAccountParameters(new JsonCredentialParameters
                {
                    PrivateKey = Configuration["GoogleSpreadsheetsCredentials:PrivateKey"],
                    ClientEmail = Configuration["GoogleSpreadsheetsCredentials:ClientEmail"],
                    ClientId = Configuration["GoogleSpreadsheetsCredentials:ClientId"],
                    Type = Configuration["GoogleSpreadsheetsCredentials:Type"]
                }, Configuration["GoogleSpreadsheetsCredentials:ApplicationName"], SheetsService.Scope.Spreadsheets)
            );

            services.AddPosStateSettingsProvider(ConfigurationReader);
            services.AddPosStateServices(ConfigurationReader);

            services.AddTransient<IPosConnectionStatusNotifications, PosConnectionStatusNotifications>();

            services.AddCheckOnlineIntegration(url: Configuration["CheckOnline:Url"],
                                               login: Configuration["CheckOnline:Login"],
                                               password: Configuration["CheckOnline:Password"],
                                               certificate: Configuration["CheckOnline:Certificate"],
                                               certificatePassword: Configuration["CheckOnline:CertificatePassword"],
                                               taxCode: int.Parse(Configuration["CheckOnline:TaxCode"]));

            services.AddPosDiagnostics(inventoryTimeoutInSeconds: int.Parse(Configuration["DailyPosDiagnostic:InventoryTimeoutInSeconds"]),
                                       doorStateTimeoutInSeconds: int.Parse(Configuration["DailyPosDiagnostic:DoorStateTimeoutInSeconds"]),
                                       includeDoorsStateCheck: bool.Parse(Configuration["DailyPosDiagnostic:IncludeDoorsStateCheck"]));

            services.AddPosDisplayActivityMonitor(ConfigurationReader);

            services.AddTransient<IAccountService, AccountService>();

            services.AddTransient<IPaymentDescriptionPrinter, PaymentRussianDescriptionPrinter>();

            services.AddSingleton<IPosSensorControllerTypeConfigurationProvider>(sp => new PosSensorControllerTypeConfigurationProvider(
                positionConfigurationCacheDurabilityInMinutes: int.Parse(Configuration["Sensors:PositionConfigurationCacheDurabilityInMinutes"])));

            services.AddPosSensorsMeasurementsManagement(ConfigurationReader);

            services.AddTransient<ICheckCorrectnessStatusProcessor, CheckCorrectnessStatusProcessor>();

            services.AddTransient<ICheckService, CheckService>();

            services.AddTacticalDiagnostics(ConfigurationReader);

            services.AddDailyPosLogsRequestsAgent(int.Parse(Configuration["PosLogs:StoreForDays"]));

            services.AddConditionalPurchaseAgent(TimeSpan.FromMinutes(int.Parse(
                Configuration[
                    "ConditionalPurchase:MinDeltaTimeInMinutesFromLabelAppearanceForDeletion"])));

            services.AddCheckManager();

            services.AddPosNotificationsAgent();

            services.AddUntiedLabeledGoodsAgent(ConfigurationReader);

            services.AddTransient<IOneCSyncService, OneCSyncService>();
            services.AddTransient<IDiscountsCheckManager, DiscountsCheckManager>();
            services.AddAbnormalUserBonusPointsSeekingAgent(decimal.Parse(Configuration["UsersBonus:MaxAmount"]));

            services.AddPosOperationsAppService();
            services.AddConfigurationManager();

            services.AddUserLogsManager();

            services.AddFiscalizationInfosService("image/png");

            services.AddWebSocketGroupHandler();

            services.AddFiscalizationInfosQrCodeConversionServices(ConfigurationReader);

            services.AddSwaggerDocument("Nasladdin API");

            services.AddPosScreenResolutionUpdater();

            services.AddVerificationCodeByTypeSeeker(ConfigurationReader);

            services.AddSingleton<IPurchaseInitiationLogger, PurchaseInitiationLogger>();

            services.AddTransient<IFiscalizationCheckPrinter, FiscalizationCheckPrinter>();

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));
        }

        public void Configure(
            IApplicationBuilder app,
            NasladdinWebSocketDuplexEventMessageHandler nasladdinWebSocketDuplexEventMessageHandler,
            IUnpaidPurchasesMonitor unpaidPurchasesMonitor,
            ILogger logger)
        {
            nasladdinWebSocketDuplexEventMessageHandler.Logger = logger;

            app.CreateRoles();
            app.CreateUserAccount(
                userName: Configuration["Data:AdminUser:Name"],
                email: Configuration["Data:AdminUser:Email"],
                password: Configuration["Data:AdminUser:Password"],
                role: Configuration["Data:AdminUser:Role"]
            );

            app.UseUserLogsManager(ConfigurationReader);

            if (!IsDevelopment)
            {
                app.UseFeedbacksTelegramNotifications(
                    chatId: long.Parse(Configuration["Telegram:NasladdinChannelChatId"])
                );
                app.UseFeedbacksEmailNotifications(
                    "Отзыв покупателя Nasladdin",
                    new Collection<string>
                    {
                        "nasladdin.club@yandex.ru"
                    }
                );

                app.UseOverdueGoodsEmailNotifications(
                    "Просроченные товары Nasladdin",
                    new Collection<string>
                    {
                        "nasladdin.club@yandex.ru"
                    }
                );
            }

            app.UsePosTokenManager(ConfigurationReader);

            app.UseOverdueGoodsTelegramNotifications();
            app.UseTacticalDiagnosticsTelegramNotifications();
            app.UseTacticalDiagnosticsLogging();
            app.RunTacticalDiagnostics(ConfigurationReader);

            app.UsePosLabeledGoodsBlockerTelegramNotifications();

            app.UseSerilog();

            app.UsePurchaseManagerLogging();

            app.UsePurchaseManagerPushNotifications();

            app.UseSmsMessageSenderTelegramNotifications();

            app.UseOverdueGoodsChecker();

            app.UseBankingCardConfirmationServiceLogging();

            app.UseExceptionHandlerWithLogging();

            app.UseStaticFiles();

            app.UseSwaggerUi(ConfigurationReader);

            app.UseCors(options =>
            {
                options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseSerilogUserIdLogging();

            app.UseSerilogRequestIdLogging();

            app.UseSerilogRequestResponseLogging(loggingPermissions: new ILoggingPermission[]
            {
                new ForbidTooLongContentLogging(),
                new ForbidRequestResponseLoggingForUriPaths(
                    Routes.TokenEndpoint,
                    "api/logs",
                    "api/account/verifyPhoneNumber"
                )
            });

            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            });

            app.UseMvc();

            app.MapWebSocketManager(ConfigurationReader.GetPlantControllerPostfixUrl());

            app.UseOngoingPurchaseActivityMonitor();

            app.UseWebSocketKeepAliveManager(keepAliveMessagesUpdatePeriod: TimeSpan.FromSeconds(15));

            app.ListenToPosWebSocketDisconnectionAndUpdatePosDisplayPage();

            app.UsePromotionNotifications();

            unpaidPurchasesMonitor.Start();

            app.UseLogsReportsRunner();
            app.RunReportsAgent(ConfigurationReader);
            app.RunPointsOfSaleReportAgent(ConfigurationReader);

            app.UseSpreadsheetUploaderLogging();
            app.UseSpreadsheetUploaderTelegramNotifications();

            app.UsePosTemperatureNotifications(ConfigurationReader);

            app.UseCheckOnlineAgent();

            app.UsePosDiagnosticsReport();
            app.RunPosDiagnosticsAgent(TasksAgentOptions.DailyStartAtFixedTime(
                TimeSpan.Parse(Configuration["DailyPosDiagnostic:DiagnosticStartingMoscowTime"])));

            app.UsePosComponentsInactiveTelegramNotification();

            app.UsePosDisplaysActivityMonitor();

            app.UsePosNotificationsAgentTelegramNotifications();
            app.RunPosNotificationsAgent(TasksAgentOptions.FixedPeriodOfTime(TimeSpan.FromMinutes(
                int.Parse(
                    Configuration["PointsOfSaleNotifications:PosDisabledNotificationsReminderIntervalInMinutes"]))));

            app.UsePosLogsRequestsAgent();

            app.UseConditionalPurchaseManager();
            app.RunConditionalPurchaseAgent(TasksAgentOptions.FixedPeriodOfTime(TimeSpan.FromMinutes(int.Parse(
                Configuration["ConditionalPurchase:DelayIntervalInMinutes"]))));

            app.UsePosDisplayServices();

            app.UseUntiedLabeledGoodsAgentTelegramNotifications();
            app.RunUntiedLabeledGoodsAgent(ConfigurationReader);

            app.UseAbnormalUsersBonusPointsTelegramAlerts();
            app.RunAbnormalUserBonusPointsSeekingAgent(TasksAgentOptions.DailyStartAtFixedTime(
                TimeSpan.Parse(Configuration["UsersBonus:AbnormalAmountSeekingStartMoscowTime"])));

            app.RunPointsOfSaleStateHistoricalDataDeletingAgent(ConfigurationReader);
        }

        private void ApplyMigrations()
        {
            using (var context = ApplicationDbContextFactory.Create())
            {
                context.Database.SetCommandTimeout(TimeSpan.FromHours(4));
                context.Database.Migrate();
            }
        }
    }
}
