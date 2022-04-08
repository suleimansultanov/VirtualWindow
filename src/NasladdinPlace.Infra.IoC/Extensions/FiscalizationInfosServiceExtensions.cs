using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Application.Services.FiscalizationInfos;
using NasladdinPlace.Application.Services.FiscalizationInfos.Contracts;
using NasladdinPlace.Core;
using NasladdinPlace.QrCodeConverter.Encoder;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class FiscalizationInfosServiceExtensions
    {
        public static void AddFiscalizationInfosService(this IServiceCollection services, string qrCodeMimeType)
        {
            if (string.IsNullOrWhiteSpace(qrCodeMimeType))
                throw new ArgumentNullException(nameof(qrCodeMimeType));

            services.AddTransient<IFiscalizationInfosService>(sp =>
            {
                var unitOfWorkFactory = sp.GetRequiredService<IUnitOfWorkFactory>();
                var qrCodeEncoder = sp.GetRequiredService<IQrCodeEncoder>();

                return new FiscalizationInfosService(unitOfWorkFactory, qrCodeEncoder, qrCodeMimeType);
            });
        }
    }
}