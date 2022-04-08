using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Core;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Reporter;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Runner.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Scheduler;
using NasladdinPlace.Core.Services.Diagnostics.TacticalDiagnostics;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using Serilog;
using System;
using CloudPaymentsClient.Domain.Services;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.Diagnostics.Extensions
{
    public static class DiagnosticsExtension
    {
	    public static void AddTacticalDiagnostics( this IServiceCollection services,
		    IConfigurationReader configurationReader )
	    {
		    if ( configurationReader == null )
			    throw new ArgumentNullException( nameof( configurationReader ) );

		    var phoneNumber = configurationReader.GetPhoneNumber();
		    var bankingCardCryptogram = configurationReader.GetBankingCardCryptogram();
		    var posQrCode = configurationReader.GetPosQrCode();

		    var tacticalDiagnosticsParams = new TacticalDiagnosticsParams( phoneNumber, bankingCardCryptogram,
			    posQrCode, new TestServiceInfo() );

		    services.AddSingleton<IDiagnosticsRunnerFactory>( sp =>
		    {
			    var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
			    tacticalDiagnosticsParams.IsDevelopmentModeEnabled = hostingEnvironment.IsDevelopment();
			    return new TacticalDiagnosticsRunnerFactory( tacticalDiagnosticsParams );
		    } );

		    services.AddSingleton<IDiagnosticsReporter, TacticalDiagnosticsFailuresReporter>();

		    services.AddSingleton<IDiagnostics, Core.Services.Diagnostics.Infrastructure.Core.Diagnostics>();

		    services.AddSingleton<IDiagnosticsScheduler, DiagnosticsScheduler>();
	    }

	    public static void RunTacticalDiagnostics( this IApplicationBuilder app,
		    IConfigurationReader configurationReader )
	    {
		    if ( configurationReader == null )
			    throw new ArgumentNullException( nameof( configurationReader ) );

		    var startMoscowTime = configurationReader.GetStartMoscowTime();

		    var options = TasksAgentOptions.DailyStartAtFixedTime( startMoscowTime );

		    var serviceProvider = app.ApplicationServices;

		    var diagnosticsScheduler = serviceProvider.GetRequiredService<IDiagnosticsScheduler>();

		    diagnosticsScheduler.ScheduleDiagnostics( options );
	    }

	    public static void UseTacticalDiagnosticsTelegramNotifications(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var telegramMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();
            var diagnostics = serviceProvider.GetRequiredService<IDiagnostics>();

            diagnostics.DiagnosticsCompleted += (sender, diagnosticsReport) =>
            {
                telegramMessageSender.SendAsync(diagnosticsReport.DiagnosticsResultDescription);
            };
        }

        public static void UseTacticalDiagnosticsLogging(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            
            var diagnostics = serviceProvider.GetRequiredService<IDiagnostics>();
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var tacticalDiagnosticsResultLogger = new TacticalDiagnosticsResultLogger(logger);

            diagnostics.DiagnosticsCompleted += (sender, diagnosticsReport) =>
            {
                tacticalDiagnosticsResultLogger.Log(diagnosticsReport.DiagnosticsResult);
            };
        }
    }
}