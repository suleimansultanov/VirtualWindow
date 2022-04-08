using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Factory;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers;
using NasladdinPlace.CheckOnline.Infrastructure;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.CheckOnline.Tools;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.CheckOnline.CheckOnlineAgent;
using NasladdinPlace.Core.Services.CheckOnline.Helpers;
using NasladdinPlace.Logging;

namespace NasladdinPlace.UI.Services.CheckOnline
{
    public static class CheckOnlineExtensions
    {
        public static void AddCheckOnlineIntegration(this IServiceCollection services,
            string url,
            string login,
            string password,
            string certificate,
            string certificatePassword,
            int taxCode)
        {
            services.AddTransient<IHttpWebRequestProvider, HttpWebRequestProvider>();
            services.AddSingleton<IOnlineCashierAuth>(sp => new CheckOnlineAuth
            {
                ServiceUrl = url,
                Login = login,
                Password = password,
                CertificateData = certificate,
                CertificatePassword = certificatePassword
            });
            services.AddSingleton<IPosOperationTransactionTypeProvider, PosOperationTransactionTypeProvider>();
            services.AddTransient<ICheckOnlineRequestProvider, CheckOnlineRequestProvider>();
            services.AddTransient(sp =>
                CheckOnlineBuilderFactory.Create(sp.GetRequiredService<ICheckOnlineRequestProvider>()));
            services.AddTransient<ICheckOnlineManager>(sp => new CheckOnlineManager(
                sp.GetRequiredService<IUnitOfWorkFactory>(),
                sp.GetRequiredService<ICheckOnlineBuilder>(),
                sp.GetRequiredService<IOnlineCashierAuth>(),
                sp.GetRequiredService<IPosOperationTransactionTypeProvider>(),
                sp.GetRequiredService<ILogger>(),
                taxCode
            ));
            services.AddTransient<ICheckOnlineAgent, CheckOnlineAgent>();
        }
    }
}
