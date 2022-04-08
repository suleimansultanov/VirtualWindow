using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.Client.Contracts;
using NasladdinPlace.Api.Client.Rest.Client.Factory;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Services.AuthTokenManagement;

namespace NasladdinPlace.UI.Extensions
{
    public static class RestClientExtensions
    {
        public static void AddRestClient(this IServiceCollection services, string baseApiUrl)
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
                throw new ArgumentNullException(nameof(baseApiUrl));
            
            services.AddTransient<IAuthTokenManager, AuthTokenManager>();
            services.AddSingleton(sp => RestClientFactory.Create(
                baseApiUrl: baseApiUrl,
                authTokenRetriever: sp.GetRequiredService<IAuthTokenManager>()
            ));
        }

        public static void UseLoggingOutOnRestClientUnauthorizedError(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var restClient = serviceProvider.GetRequiredService<IRestClient>();
            restClient.OnUnauthorizedError += async (sender, args) => { await LogOutAsync(serviceProvider); };
        }

        private static async Task LogOutAsync(IServiceProvider serviceProvider)
        {
            var authTokenManager = serviceProvider.GetRequiredService<IAuthTokenManager>();
            
            await authTokenManager.RemoveAuthTokenAsync();

            using (var scope = serviceProvider.CreateScope())
            {
                var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                await signInManager.SignOutAsync();
            }
        }
    }
}