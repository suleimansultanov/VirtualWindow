using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Manager.Contracts;
using NasladdinPlace.Core.Services.Configuration.Manager.Models;
using NasladdinPlace.Core.Services.Configuration.Validators.Factory;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Configuration.Manager
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfigurationValueValidatorsFactory _configurationValueValidatorsFactory;

        public ConfigurationManager(
            IUnitOfWorkFactory unitOfWorkFactory,
            IConfigurationValueValidatorsFactory configurationValueValidatorsFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (configurationValueValidatorsFactory == null)
                throw new ArgumentNullException(nameof(configurationValueValidatorsFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
            _configurationValueValidatorsFactory = configurationValueValidatorsFactory;
        }

        public Task<Result> TryCreateKeyIfNotExistsAndSetValueAsync(
            ConfigurationKeyCreationInfo configurationKeyCreationInfo, string value)
        {
            return TryCreateKeyIfNotExistsAndProvidedAndSetValueAsync(
                provideConfigurationKey: configurationKeyCreationInfo.ToConfigurationKey,
                keyIdentifier: configurationKeyCreationInfo.KeyIdentifier,
                value: value
            );
        }

        public Task<Result> TrySetValueAsync(ConfigurationKeyIdentifier keyIdentifier, string value)
        {
            return TryCreateKeyIfNotExistsAndProvidedAndSetValueAsync(
                provideConfigurationKey: () => null,
                keyIdentifier: keyIdentifier,
                value: value
            );
        }

        public async Task<ValueResult<string>> TryGetValueByKeyAsync(ConfigurationKeyIdentifier keyIdentifier)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var configurationKey = await unitOfWork.ConfigurationKeys.GetByIdIncludingValuesAsync(keyIdentifier);

                return configurationKey == null
                    ? ValueResult<string>.Failure("Configuration key has not been found.")
                    : ValueResult<string>.Success(configurationKey.SingleValue().Value);
            }
        }

        private Task<Result> TryCreateKeyIfNotExistsAndProvidedAndSetValueAsync(
            Func<ConfigurationKey> provideConfigurationKey, ConfigurationKeyIdentifier keyIdentifier, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            return TryCreateKeyIfNotExistsAndProvidedAndSetValueAuxAsync(provideConfigurationKey, keyIdentifier, value);
        }

        private async Task<Result> TryCreateKeyIfNotExistsAndProvidedAndSetValueAuxAsync(
            Func<ConfigurationKey> provideConfigurationKey, ConfigurationKeyIdentifier keyIdentifier, string value)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var configurationKey = await unitOfWork.ConfigurationKeys.GetByIdIncludingValuesAsync(keyIdentifier);

                if (configurationKey == null)
                {
                    configurationKey = provideConfigurationKey();


                    if (configurationKey == null) return Result.Failure();

                    unitOfWork.ConfigurationKeys.Add(configurationKey);
                }

                var configurationValue = new ConfigurationValue(keyIdentifier, value);

                var isSettingValueSuccessful =
                    configurationKey.TrySetValue(configurationValue, _configurationValueValidatorsFactory);

                if (isSettingValueSuccessful)
                {
                    await unitOfWork.CompleteAsync();
                }

                return isSettingValueSuccessful ? Result.Success() : Result.Failure();
            }
        }
    }
}