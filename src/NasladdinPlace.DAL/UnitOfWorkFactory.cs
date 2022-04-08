using NasladdinPlace.Core;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;

namespace NasladdinPlace.DAL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IApplicationDbContextFactory _applicationDbContextFactory;
        private readonly IPosRealTimeInfoDataStore _posRealTimeInfoDataStore;

        public UnitOfWorkFactory(
            IApplicationDbContextFactory applicationDbContextFactory,
            IPosRealTimeInfoDataStore posRealTimeInfoDataStore)
        {
            _applicationDbContextFactory = applicationDbContextFactory;
            _posRealTimeInfoDataStore = posRealTimeInfoDataStore;
        }


        public IUnitOfWork MakeUnitOfWork()
        {
            return new UnitOfWork(_applicationDbContextFactory.Create(), _posRealTimeInfoDataStore);
        }
    }
}