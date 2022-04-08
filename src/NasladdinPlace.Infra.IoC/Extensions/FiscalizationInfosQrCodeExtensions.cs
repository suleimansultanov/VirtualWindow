using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.QrCodeConverter.Encoder;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class FiscalizationInfosQrCodeExtensions
    {
        public static void AddFiscalizationInfosQrCodeConversionServices(
            this IServiceCollection services, IConfigurationReader configurationReader)
        {
	        var qrCodeDimensionSize = configurationReader.GetFiscalizationQrCodeDimensionSize();
            
            services.AddTransient<IQrCodeEncoder>(sp => new QrCodeEncoder(qrCodeDimensionSize));
        }
    }
}