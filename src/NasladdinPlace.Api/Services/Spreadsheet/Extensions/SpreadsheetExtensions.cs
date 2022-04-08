using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Spreadsheet.Creators;
using NasladdinPlace.Api.Services.Spreadsheet.Creators.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Factories;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Models;
using NasladdinPlace.Api.Services.Spreadsheet.Providers;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Uploader;
using NasladdinPlace.Api.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Models;
using NasladdinPlace.Spreadsheets.Providers;
using NasladdinPlace.Spreadsheets.Services.Creators;
using NasladdinPlace.Spreadsheets.Services.Creators.Contracts;
using NasladdinPlace.Spreadsheets.Services.Credential;
using NasladdinPlace.Spreadsheets.Services.Credential.Contracts;
using NasladdinPlace.Spreadsheets.Services.Fetcher;
using NasladdinPlace.Spreadsheets.Services.Fetcher.Contracts;
using NasladdinPlace.Spreadsheets.Services.Formatters;
using NasladdinPlace.Spreadsheets.Services.Formatters.Contracts;
using Serilog;

namespace NasladdinPlace.Api.Services.Spreadsheet.Extensions
{
    public static class SpreadsheetExtensions
    {
	    public static void AddScheduledSpreadsheetsUploader( this IServiceCollection services,
		    IConfigurationReader configurationReader,
		    GoogleServiceAccountParameters googleServiceAccountParameters )
	    {
		    services.AddSingleton<IGoogleCredential>( sp =>
			    new GoogleServiceAccountCredential( googleServiceAccountParameters.CredentialParameters,
				    googleServiceAccountParameters.Scopes ) );

		    services.AddSingleton<ISpreadsheetIdFetcher, SpreadsheetGoogleIdFromUrlFetcher>();
		    services.AddSingleton<ISpreadsheetCellFormatter, SpreadsheetCellFormatter>();
		    services.AddSingleton<ISpreadsheetDataRangeCreator, SpreadsheetDataRangeCreator>();

		    services.AddSingleton<ISpreadsheetProvider>( sp =>
			    new SpreadsheetProvider(
				    sp.GetRequiredService<IGoogleCredential>(),
				    sp.GetRequiredService<ISpreadsheetIdFetcher>(),
				    sp.GetRequiredService<ISpreadsheetCellFormatter>(),
				    sp.GetRequiredService<ISpreadsheetDataRangeCreator>(),
				    googleServiceAccountParameters.ApplicationName ) );

		    services.AddSingleton<IReportDataBatchProviderFactory>( sp =>
			    new ReportDataBatchProviderFactory( sp.GetRequiredService<IServiceProvider>() ) );

		    services.AddSingleton<IReportFieldConvertsFactory>( sp =>
			    new ReportFieldConvertsFactory() );
		    services.AddSingleton<IPurchaseReportRecordFactory, PurchaseReportRecordFactory>();
		    services.AddSingleton<IReportDataBatchProviderFactory, ReportDataBatchProviderFactory>();
		    services.AddSingleton<IPurchaseReportRecordsCreator, PurchaseReportRecordsCreator>();

		    var delayBeforeRetryInMinutes = configurationReader.GetSpreadsheetsUploaderDelayBeforeRetryInMinutes();
		    var permittedRetryCount = configurationReader.GetSpreadsheetsUploaderPermittedRetryCount();
		    var reportDataExportingPeriodInDays = configurationReader.GetReportDataExportingPeriodInDays();

		    services.AddSingleton<ISpreadsheetsUploader>( sp => new SpreadsheetsUploader(
			    sp.GetRequiredService<IUnitOfWorkFactory>(),
			    sp.GetRequiredService<ISpreadsheetProvider>(),
			    sp.GetRequiredService<IReportDataBatchProviderFactory>(),
			    new SpreadsheetsUploadingTaskParameters( delayBeforeRetryInMinutes, permittedRetryCount,
				    reportDataExportingPeriodInDays ),
			    sp.GetRequiredService<NasladdinPlace.Logging.ILogger>() ) );

		    var uri = configurationReader.GetApproachesDataDocumentUri();
		    var sheet = configurationReader.GetApproachesDataDocumentSheetName();
		    var cacheLifeTimeInSeconds = configurationReader.GetApproachesDataCacheLifeTimeInSeconds();

		    services.AddSingleton<IApproachesUploader>( sp => new ApproachesUploader(
			    sp.GetRequiredService<ISpreadsheetProvider>(),
			    new SpreadsheetsUploadingTaskParameters(
				    delayBeforeRetryInMinutes,
				    permittedRetryCount,
				    reportDataExportingPeriodInDays ),
			    uri,
			    sheet ) );

		    services.AddSingleton<IApproachesHolderProvider>( sp => new ApproachesHolderProvider(
			    sp.GetRequiredService<IApproachesUploader>(),
			    TimeSpan.FromSeconds( cacheLifeTimeInSeconds ) ) );
	    }

	    public static void UseSpreadsheetUploaderLogging(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            AddEventHandler(app,  (error) =>
            {
                var logger = services.GetRequiredService<ILogger>();
                logger.Error($"An error occurred while attempting to upload a spreadsheet {error.Type} " +
                              $"because {error.Message}.");
            });
        }

        public static void UseSpreadsheetUploaderTelegramNotifications(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var telegramChannelMessageSender = services.GetRequiredService<ITelegramChannelMessageSender>();

            AddEventHandler(app, async (error) =>
            {
                string messageDescriptionStatusCode;

                switch (error.Code)
                {
                    case 403:
                        messageDescriptionStatusCode = "нет доступа к выбраному файлу";
                        break;
                    case 404:
                        messageDescriptionStatusCode = "файл не найден";
                        break;
                    case 500:
                        messageDescriptionStatusCode = "внутренняя ошибка сервера";
                        break;
                    case 503:
                        messageDescriptionStatusCode = "сервер не доступен";
                        break;
                    default:
                        messageDescriptionStatusCode = $"{error.Code}: {error.Message}";
                        break;
                }

                var errorMessage = $"При выгрузке отчета \"{error.Type}\" произошла ошибка \"{messageDescriptionStatusCode}\".";

                await telegramChannelMessageSender.SendAsync(errorMessage);
            });
        }

        private static void AddEventHandler(IApplicationBuilder app, Action<ReportUploadingError> errorMessageHandler)
        {
            var services = app.ApplicationServices;

            var spreadsheetsUploader = services.GetRequiredService<ISpreadsheetsUploader>();

            spreadsheetsUploader.ErrorHandler += (sender, error) => { errorMessageHandler(error); };
        }
    }
}
