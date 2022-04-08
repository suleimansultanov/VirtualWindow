using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Reader;

namespace NasladdinPlace.Api.Tests.Utils
{
    public class FakeConfigurationReader : IConfigurationReader
    {
        public void LoadConfiguration()
        {
            // do nothing
        }

        public void UnloadConfiguration()
        {
            // do nothing
        }

        public bool TryGetValueByKey<T>(ConfigurationKeyIdentifier keyIdentifier, out T value)
        {
            switch (keyIdentifier)
            {
				case ConfigurationKeyIdentifier.BaseApiUrl:
					value = (T) (object) "http://api.com";
					break;
                case ConfigurationKeyIdentifier.FiscalizationQrCodeDimensionSize:
                    value = (T) (object) 256;
                    break;
                case ConfigurationKeyIdentifier.FirebaseCloudMessagingApiUrl:
                    value = (T)(object)"https://fcm.googleapis.com";
                    break;
                case ConfigurationKeyIdentifier.FirebaseTokenApiUrl:
                    value = (T)(object)"https://iid.googleapis.com";
                    break;
                case ConfigurationKeyIdentifier.NextPaymentAttemptTimeout:
                    value = (T)(object) TimeSpan.FromMinutes(1);
                    break;
                case ConfigurationKeyIdentifier.CatalogPageSize:
                    value = (T)(object)2;
                    break;
                case ConfigurationKeyIdentifier.CategoriesPageSize:
                    value = (T)(object)1;
                    break;
                case ConfigurationKeyIdentifier.DefaultImagePath:
                    value = (T)(object) "defailt-image.webp";
                    break;
                default:
                    value = default(T);
                    return false;
            }

            return true;
        }

        public bool TryGetValuesByKey<T>(ConfigurationKeyIdentifier keyIdentifier, out IEnumerable<T> values)
        {
            values = new Collection<T>();
            return false;
        }
    }
}