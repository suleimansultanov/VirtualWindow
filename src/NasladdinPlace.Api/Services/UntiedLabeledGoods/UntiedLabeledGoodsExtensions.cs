using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Agent;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Agent.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Contracts;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Formatters.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Api.Services.UntiedLabeledGoods
{
    public static class UntiedLabeledGoodsExtensions
    {
	    public static void AddUntiedLabeledGoodsAgent( this IServiceCollection services,
		    IConfigurationReader configurationReader )
	    {
            var adminBaseUrl = configurationReader.GetAdminPageBaseUrl();

            var identificationAdminPageBaseUrlFormat = ConfigurationReaderExt.CombineUrlParts(adminBaseUrl,configurationReader.GetIdentificationAdminPageBaseUrlFormat());
		    var documentGoodsMovingPageUrlFormat = ConfigurationReaderExt.CombineUrlParts(adminBaseUrl,configurationReader.GetDocumentGoodsMovingPageUrlFormat());

		    services.AddSingleton<IPrintedUntiedLabeledGoodsFormatter>( sp =>
			    new PrintedUntiedLabeledGoodsFormatter( identificationAdminPageBaseUrlFormat ) );
		    services.AddSingleton<IPrintDocumentGoodsMovingUntiedLabeledGoodsFormatter>( sp =>
			    new PrintDocumentGoodsMovingUntiedLabeledGoodsFormatter( documentGoodsMovingPageUrlFormat ) );

		    services.AddSingleton<IUntiedLabeledGoodsInfoMessagePrinter, UntiedLabeledGoodsInfoRussianPrinter>();
		    services.AddSingleton<IUntiedLabeledGoodsManager>( sp =>
			    new UntiedLabeledGoodsManager( sp.GetRequiredService<IUnitOfWorkFactory>(),
				    sp.GetRequiredService<ILogger>() ) );
		    services.AddSingleton<IUntiedLabeledGoodsAgent>( sp => new UntiedLabeledGoodsAgent(
			    sp.GetRequiredService<ITasksAgent>(),
			    sp.GetRequiredService<IUntiedLabeledGoodsManager>() ) );
	    }

	    public static void UseUntiedLabeledGoodsAgentTelegramNotifications(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var telegramMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();
            var untiedLabeledGoodsManager = serviceProvider.GetRequiredService<IUntiedLabeledGoodsManager>();
            var untiedLabeledGoodsInfoMessagePrinter = serviceProvider.GetRequiredService<IUntiedLabeledGoodsInfoMessagePrinter>();

            untiedLabeledGoodsManager.OnUntiedLabeledGoodsFound += async (sender, untiedLabeledGoodsInfos) =>
            {
                await telegramMessageSender.SendAsync(
                    untiedLabeledGoodsInfoMessagePrinter.Print(untiedLabeledGoodsInfos));
            };
        }

        public static void RunUntiedLabeledGoodsAgent(this IApplicationBuilder app, IConfigurationReader configurationReader)
        {
	        var recheckIntervalInMinutes = configurationReader.GetRecheckIntervalInMinutes();

            var options = TasksAgentOptions.FixedPeriodOfTime(TimeSpan.FromMinutes(recheckIntervalInMinutes));
            var serviceProvider = app.ApplicationServices;
            var untiedLabeledGoodsAgent = serviceProvider.GetRequiredService<IUntiedLabeledGoodsAgent>();
            
            untiedLabeledGoodsAgent.Start(options);
        }
    }
}