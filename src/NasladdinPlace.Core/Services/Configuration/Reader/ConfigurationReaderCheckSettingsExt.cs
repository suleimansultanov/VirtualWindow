using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderCheckSettingsExt
	{
		public static string GetFiscalizationQrCodeUrlTemplate( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.FiscalizationQrCodeUrlTemplate );
		}

		public static string GetFiscalCheckUrlTemplate( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.FiscalCheckUrlTemplate );
		}

		public static string GetRefundMessageFormat( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.RefundMessageFormat );
		}

		public static bool GetIsPermittedToNotifyAboutRefund( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<bool>( ConfigurationKeyIdentifier.IsPermittedToNotifyAboutRefund );
		}

		public static string GetAdditionOrVerificationMessageFormat( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<string>( ConfigurationKeyIdentifier.AdditionOrVerificationMessageFormat );
		}

		public static bool GetIsPermittedToNotifyAboutAdditionOrVerification( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<bool>( ConfigurationKeyIdentifier.IsPermittedToNotifyAboutAdditionOrVerification );
		}

		public static int GetFiscalizationQrCodeDimensionSize( this IConfigurationReader configurationReader ) {
			return configurationReader.GetValueByKey<int>( ConfigurationKeyIdentifier.FiscalizationQrCodeDimensionSize );
        }
        public static TimeSpan GetNextPaymentAttemptTimeout(this IConfigurationReader configurationReader)
        {
            return configurationReader.GetValueByKey<TimeSpan>(ConfigurationKeyIdentifier.NextPaymentAttemptTimeout);
        }
    }
}