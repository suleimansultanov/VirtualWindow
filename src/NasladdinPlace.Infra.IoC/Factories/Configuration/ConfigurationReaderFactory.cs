using System;
using NasladdinPlace.Core.Services.Configuration.Parser.Factory;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.DAL;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;

namespace NasladdinPlace.Infra.IoC.Factories.Configuration
{
    public static class ConfigurationReaderFactory
    {
        public static IConfigurationReader Create(IApplicationDbContextFactory applicationDbContextFactory)
        {
            if (applicationDbContextFactory == null)
                throw new ArgumentNullException(nameof(applicationDbContextFactory));
            
            var posRealTimeInfoDataStore = new StubPosRealmInfoDataStore();
            var unitOfWorkFactory = new UnitOfWorkFactory(
                applicationDbContextFactory,
                posRealTimeInfoDataStore
            );
            
            return new ConfigurationReader(unitOfWorkFactory, new ConfigurationValueParsersFactory());
        }
    }
}