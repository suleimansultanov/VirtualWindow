using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;

namespace NasladdinPlace.Api.Services.OngoingPurchase
{
    public static class OngoingPurchaseExtensions
    {
        public static void UseOngoingPurchaseActivityMonitor(this IApplicationBuilder app)
        {
            var ongoingPurchaseActivityMonitor =
                app.ApplicationServices.GetRequiredService<IOngoingPurchaseActivityMonitor>();
            ongoingPurchaseActivityMonitor.Start();
        }
    }
}