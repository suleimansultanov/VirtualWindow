using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NasladdinPlace.Logging.Mvc.Middleware.RequestResponse;
using NasladdinPlace.Logging.Mvc.Middleware.RequestResponse.LoggingPermission;
using NasladdinPlace.Logging.Serilog.Constants;
using NasladdinPlace.Logging.Serilog.LoggerSinks;
using NasladdinPlace.Logging.Serilog.LoggerSinks.Helpers;
using NasladdinPlace.Logging.Serilog.Middleware;
using NasladdinPlace.Logging.Serilog.Wrapper;
using NasladdinPlace.Logging.Storage;
using NasladdinPlace.Logging.Writers;
using NasladdinPlace.Utilities.Buffer;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using ILogger = NasladdinPlace.Logging.ILogger;
using Log = NasladdinPlace.Logging.Models.Log;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class SerilogExtensions
    {
        private const string LogFormat =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}]" +
            "[RequestId={" + SerilogPropertyNames.RequestId + "}]" +
            "[UserId={" + SerilogPropertyNames.UserId + "}] " +
            "{Message:lj}" +
            "{NewLine}" +
            "{Exception}";

        public static void AddSerilog(this IServiceCollection services, Func<IServiceProvider, ILogWriter> logWriterViaWebSocket)
        {
            AddSerilogAux(services, logWriterViaWebSocket);
        }

        public static void AddSerilog(this IServiceCollection services)
        {
            AddSerilogAux(services, _ => new NowhereLogWriter());
        }

        public static void UseSerilog(this IApplicationBuilder app)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(CloseAndFlushLoggerOnAppStopping);
        }

        public static void UseSerilogRequestIdLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestIdLoggingMiddleware>();
        }

        public static void UseSerilogUserIdLogging(this IApplicationBuilder app)
        {
            var identityOptionsAccessor = app.ApplicationServices.GetRequiredService<IOptions<IdentityOptions>>();
            app.UseMiddleware<UserIdLoggingMiddleware>(identityOptionsAccessor);
        }

        public static void UseSerilogRequestResponseLogging(
            this IApplicationBuilder app,
            IEnumerable<ILoggingPermission> loggingPermissions)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>(logger, loggingPermissions);
        }

        private static void CloseAndFlushLoggerOnAppStopping()
        {
            global::Serilog.Log.CloseAndFlush();
        }

        private static void AddSerilogAux(IServiceCollection services, Func<IServiceProvider, ILogWriter> logWriterViaWebSocket)
        {
            services.AddSingleton<ILogsStorage>(sp => new BufferLogsStorage(new ConcurrentBuffer<Log>(1000)));
            services.AddTransient<InMemoryLogWriter>();
            services.AddSingleton<ILogFromLogEventFactory>(sp => new LogFromLogEventFactory(formatProvider: null));

            services.AddSingleton(sp =>
            {
                global::Serilog.Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Async(conf =>
                        conf.File(Path.Combine(sp.GetService<IHostingEnvironment>().WebRootPath, "logs/log-.txt"),
                            buffered: true,
                            fileSizeLimitBytes: null,
                            rollingInterval: RollingInterval.Day,
                            flushToDiskInterval: TimeSpan.FromSeconds(30),
                            outputTemplate: LogFormat,
                            retainedFileCountLimit: 62))
                    .WriteTo.CustomSink(
                        sp.GetRequiredService<InMemoryLogWriter>(),
                        sp.GetRequiredService<ILogFromLogEventFactory>()
                    )
                    .WriteTo.CustomSink(
                        logWriterViaWebSocket(sp),
                        sp.GetRequiredService<ILogFromLogEventFactory>()
                    )
                    .CreateLogger();
                return global::Serilog.Log.Logger;
            });

            services.AddSingleton<ILogger, SerilogLoggerWrapper>();
        }
    }
}
