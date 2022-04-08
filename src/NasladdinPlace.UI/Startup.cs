using AutoMapper;
using CloudPaymentsClient.Domain.Factories.PaymentService;
using FirebaseCloudMessagingClient.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Discounts.Managers;
using NasladdinPlace.Core.Services.Check.Extensions;
using NasladdinPlace.Core.Services.Check.Simple.Payment;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.LabeledGoods.Disabled;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.Payment.Adder;
using NasladdinPlace.Core.Services.Payment.Adder.Contracts;
using NasladdinPlace.Core.Services.Payment.Printer;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.Pos.Doors;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Extensions;
using NasladdinPlace.Core.Services.Pos.Groups.Groupers.LabeledGoods;
using NasladdinPlace.Core.Services.Pos.LabeledGoodsCreator;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers;
using NasladdinPlace.Core.Services.Pos.State;
using NasladdinPlace.Core.Services.Pos.State.Contracts;
using NasladdinPlace.Core.Services.Pos.Status;
using NasladdinPlace.Core.Services.PromotionNotifications.PromotionManager;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker;
using NasladdinPlace.Core.Services.PurchasesResetter;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Core.Services.UserBalanceCalculator;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.DAL.EntityConfigurations.Contracts;
using NasladdinPlace.DAL.EntityConfigurations.Default;
using NasladdinPlace.Infra.IoC.Extensions;
using NasladdinPlace.Infra.IoC.Factories.Configuration;
using NasladdinPlace.Infra.IoC.Factories.DbContext;
using NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission;
using NasladdinPlace.Spreadsheets.Providers;
using NasladdinPlace.Spreadsheets.Services.Creators;
using NasladdinPlace.Spreadsheets.Services.Creators.Contracts;
using NasladdinPlace.Spreadsheets.Services.Credential;
using NasladdinPlace.Spreadsheets.Services.Credential.Contracts;
using NasladdinPlace.Spreadsheets.Services.Fetcher;
using NasladdinPlace.Spreadsheets.Services.Fetcher.Contracts;
using NasladdinPlace.Spreadsheets.Services.Formatters;
using NasladdinPlace.Spreadsheets.Services.Formatters.Contracts;
using NasladdinPlace.TelegramBot.Extentions;
using NasladdinPlace.UI.Configurations;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.DependencyInjection;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Managers.Discounts;
using NasladdinPlace.UI.Resources;
using NasladdinPlace.UI.Services.Authorization;
using NasladdinPlace.UI.Services.CheckOnline;
using NasladdinPlace.UI.Services.Localization;
using NasladdinPlace.UI.Services.Localization.Providers;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.Services.PosScreenTemplates.Extensions;
using System;
using System.Globalization;
using System.Reflection;
using CloudPaymentsClient.Rest.Dtos.Payment;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Helpers;
using NasladdinPlace.Core.Services.Configuration.Helpers;
using NasladdinPlace.Core.Services.MessageSender.Sms.Extensions;
using NasladdinPlace.Core.Services.PaymentCards;
using NasladdinPlace.Core.Services.PaymentCards.Contracts;
using NasladdinPlace.Core.Services.Pos.WebSocket.Factory;
using NasladdinPlace.UI.Services.GoodCategories;
using NasladdinPlace.UI.Services.GoodCategories.Contracts;
using NasladdinPlace.UI.Services.Goods;
using NasladdinPlace.UI.Services.Goods.Contracts;
using NasladdinPlace.UI.Services.Users;

namespace NasladdinPlace.UI
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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("azurekeyvault.json", optional: true, reloadOnChange: true)
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

            ConfigurationReader.LoadConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(sp => ConfigurationReader);

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddScoped<IEntityConfigurations>(sp => new EntityConfigurations());

            services.AddScoped(sp => ApplicationDbContextFactory.Create());

            services.AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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

            services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.Cookie.Expiration = TimeSpan.FromMinutes(120);
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Login");
                });

            services.AddAppFeatures();

            services.AddTransient<ICloudPaymentsServiceFactory>(sp => new CloudPaymentsesServiceFactory(
                new ServiceInfo(
                    id: Configuration["CloudPayments:ServiceId"],
                    secret: Configuration["CloudPayments:Secret"]
                )));

            services.AddSingleton(sp => sp.GetRequiredService<ICloudPaymentsServiceFactory>().CreatePaymentService());

            services.AddSingleton(sp => sp.GetRequiredService<ICloudPaymentsServiceFactory>().CreateCloudKassirService());

            services.AddTransient<IDiscountsCheckManager, DiscountsCheckManager>();
            services.AddTransient<IFirstPayBonusAdder, FirstPayBonusAdder>();
            services.AddTransient<IPosOperationTransactionCheckItemsMaker, PosOperationTransactionCheckItemsMaker>();
            services.AddTransient<IOperationTransactionManager, OperationTransactionManager>();
            services.AddTransient<IPosOperationTransactionCreationUpdatingService, PosOperationTransactionCreationUpdatingService>();
            services.AddTransient<IOperationsManager, OperationsManager>();
            services.AddTransient<IPaymentInfoCreator, PaymentInfoCreator>();
            services.AddTransient<ICheckPaymentService, CheckPaymentService>();

            services.AddCheckManagerSmsSender(ConfigurationReader);

            services.AddTransient<IPaymentCardsService, PaymentCardsService>();

            services.AddCheckManager();

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MvcLocalizationConfiguration>());

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(DataAnnotationMessagesLocalization).GetTypeInfo()
                            .Assembly.FullName);
                        return factory.Create("DataAnnotationMessagesLocalization", assemblyName.Name);
                    };
                });

            services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();

            services.AddCors(options => options.AddPolicy(CorsPolicies.AllowFullAccess, builder =>
            {
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials();
            }));

            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(10); });

            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IPosLabeledGoodsFromLabelsCreator, PosLabeledGoodsFromLabelsCreator>();
            services.AddTransient<IPosConnectionStatusDetector, EmptyPosConnectionStatusDetector>();
            services.AddSingleton<IWsCommandsQueueProcessorFactory, WsCommandsQueueProcessorFactory>();
            services.AddSingleton<IPosRealTimeInfoDataStore, PosRealTimeInfoDataStore>();
            services.AddTransient<IPurchasesHistoryMaker, PurchasesHistoryMaker>();
            services.AddTransient<IUserPurchasesResetter, UnfinishedUserPurchasesResetter>();

            services.AddPosStateSettingsProvider(ConfigurationReader);
            services.AddTransient<IPosEquipmentStateManager, PosEquipmentStateManager>();
            services.AddSingleton<IPosDoorsStateTracker, PosDoorsStateTracker>();

            services.AddSingleton<IEntityConfigurationsFactory, EntityConfigurationsFactory>();

            services.AddSingleton(sp => ApplicationDbContextFactory);

            services.AddTelegramBot(
                apiKey: Configuration["Telegram:Bot:ApiKey"],
                telegramChannelId: ServerModeHelper.GetTelegramChannelId(ConfigurationReader, Configuration));

            services.AddPosLabeledGoodsBlocker();

            services.AddDetailedCheck(ConfigurationReader);

            services.AddSimpleCheck();

            services.AddUsersUnpaidOperationsChecksMaker();

            services.AddScoped<IUserPaymentBalanceCalculator, UserPaymentBalanceCalculator>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddRestClient(baseApiUrl: ConfigurationReader.GetBaseApiUrl());
            services.AddSingleton<INasladdinApiClient, NasladdinApiClient>();
            services.AddSingleton(sp => Configuration);

            services.AddTransient<ILabeledGoodsByPosGrouper, LabeledGoodsByPosGrouper>();
            services.AddTransient<IDisabledLabeledGoodsManager, DisabledLabeledGoodsManager>();

            services.AddUserAppFeaturesAuthorization();

            services.AddTransient<IPromotionManager, PromotionManager>();
            services.AddFirebaseCloudMessagingClient(apiKey: Configuration["Firebase:ApiKey"], configurationReader: ConfigurationReader);

            services.AddSingleton<INotificationsLogger, NotificationsLogger>();

            services.AddTransient<IGoodService, GoodService>();
            services.AddTransient<IGoodCategoryService, GoodCategoryService>();

            services.AddSingleton<IGoogleCredential>(sp =>
                new GoogleServiceAccountCredential(new JsonCredentialParameters
                {
                    PrivateKey = Configuration["GoogleSpreadsheetsCredentials:PrivateKey"],
                    ClientEmail = Configuration["GoogleSpreadsheetsCredentials:ClientEmail"],
                    ClientId = Configuration["GoogleSpreadsheetsCredentials:ClientId"],
                    Type = Configuration["GoogleSpreadsheetsCredentials:Type"]
                },
                    new[] { SheetsService.Scope.Spreadsheets }));

            services.AddSingleton<ISpreadsheetIdFetcher, SpreadsheetGoogleIdFromUrlFetcher>();
            services.AddSingleton<ISpreadsheetCellFormatter, SpreadsheetCellFormatter>();
            services.AddSingleton<ISpreadsheetDataRangeCreator, SpreadsheetDataRangeCreator>();
            services.AddSingleton<ISpreadsheetProvider>(sp =>
                new SpreadsheetProvider(
                    sp.GetRequiredService<IGoogleCredential>(),
                    sp.GetRequiredService<ISpreadsheetIdFetcher>(),
                    sp.GetRequiredService<ISpreadsheetCellFormatter>(),
                    sp.GetRequiredService<ISpreadsheetDataRangeCreator>(),
                    applicationName: Configuration["GoogleSpreadsheetsCredentials:ApplicationName"]));

            services.AddCheckOnlineIntegration(url: Configuration["CheckOnline:Url"],
                                               login: Configuration["CheckOnline:Login"],
                                               password: Configuration["CheckOnline:Password"],
                                               certificate: Configuration["CheckOnline:Certificate"],
                                               certificatePassword: Configuration["CheckOnline:CertificatePassword"],
                                               taxCode: int.Parse(Configuration["CheckOnline:TaxCode"]));

            services.AddTransient<IPaymentDescriptionPrinter, PaymentRussianDescriptionPrinter>();
            services.AddTransient<IDiscountsManager, DiscountsManager>();

            services.AddSingleton<IAuthorizationHandler, AuthorizationPermissionApiHandler>();

            services.AddTransient<IUserService, UserService>();

            services.AddSerilog();

            services.AddPosScreenTemplates(ConfigurationReader);

            services.AddSingleton<IContentTypeProvider>(new FileExtensionContentTypeProvider());

            services.AddConfigurationManager();

            services.AddSmsMessageSender(ConfigurationReader,
                ServerModeHelper.IsServerModeDevelopment(ConfigurationReader));

            services.AddTransient<IFiscalizationCheckPrinter, FiscalizationCheckPrinter>();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));
        }

        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IContentTypeProvider contentTypeProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseLoggingOutOnRestClientUnauthorizedError();
            app.UseSerilog();

            if (IsDevelopment)
            {
                app.UseDeveloperExceptionPage();

                app.UseBrowserLink();

                app.UseCors(options =>
                {
                    options
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var supportedCultures = new[]
            {
                new CultureInfo("ru")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = contentTypeProvider
            });

            app.UseAppFeatures();
            app.AssignAppFeaturesToRoles();

            app.UseAuthentication();

            app.UseSerilogRequestIdLogging();

            app.UseSerilogUserIdLogging();

            app.UseSerilogRequestResponseLogging(loggingPermissions: new ILoggingPermission[]
            {
                new ForbidTooLongContentLogging(),
                new ForbidRequestResponseLoggingForUriPaths(paths: "account/login"),
                new ForbidResponseBodyLogging()
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=PointsOfSale}/{action=All}/{id?}");
            });

            app.UseDisablingRefundedCheckItems();

            app.UseCheckManagerSmsUserNotifier();

            app.UseSmsMessageSenderTelegramNotifications();
        }
    }
}