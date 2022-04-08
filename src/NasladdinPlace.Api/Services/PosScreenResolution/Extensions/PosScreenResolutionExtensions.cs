using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.PosScreenResolution;
using NasladdinPlace.Core.Services.PosScreenResolution.Contracts;

namespace NasladdinPlace.Api.Services.PosScreenResolution.Extensions
{
    public static class PosScreenResolutionExtensions
    {
        public static void AddPosScreenResolutionUpdater(this IServiceCollection services)
        {
            services.AddSingleton<IPosScreenResolutionUpdater>(sp =>
                new PosScreenResolutionUpdater(sp.GetRequiredService<IUnitOfWorkFactory>()));
        }
    }
}