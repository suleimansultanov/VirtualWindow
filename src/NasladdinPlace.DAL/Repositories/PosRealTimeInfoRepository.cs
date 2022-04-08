using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosRealTimeInfoRepository : IPosRealTimeInfoRepository
    {   
        private readonly IPosRealTimeInfoDataStore _dataStore;
        private readonly ApplicationDbContext _context;

        public PosRealTimeInfoRepository(
            ApplicationDbContext context, 
            IPosRealTimeInfoDataStore dataStore)
        {
            if (dataStore == null)
                throw new ArgumentNullException(nameof(dataStore));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            
            _dataStore = dataStore;
            _context = context;
        }
        
        public PosRealTimeInfo GetById(int id)
        {
            var posRealTimeInfo = _dataStore.GetOrAddById(id);
            var pos = _context.PointsOfSale.Find(id);
            posRealTimeInfo.PosActivityStatus =
                pos == null ?
                    PosActivityStatus.Active :
                    pos.PosActivityStatus;
            return posRealTimeInfo;
        }

        public IEnumerable<PosRealTimeInfo> GetConnectedWithoutOrHavingVersionLessThan(string version)
        {
            return _dataStore.GetAll()
                .Where(prti => prti.Version == null || string.CompareOrdinal(prti.Version, version) < 0)
                .Where(prti => prti.ConnectionStatus == PosConnectionStatus.Connected)
                .ToImmutableList();
        }

        public IEnumerable<PosRealTimeInfo> GetAll()
        {
            return _dataStore.GetAll();
        }

        public IEnumerable<PosRealTimeInfo> GetConnected()
        {
            return _dataStore.GetAll()
                .Where(prti => prti.ConnectionStatus == PosConnectionStatus.Connected)
                .ToImmutableList();
        }

        public List<PosRealTimeInfo> GetByIds(List<int> posIds)
        {
            return _dataStore.GetAll()
                .Where(p => posIds.Contains(p.Id))
                .ToList();
        }
    }
}