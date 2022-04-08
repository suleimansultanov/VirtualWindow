using System.Collections.Generic;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.DataStore.PosRealTimeInfo
{
    public interface IPosRealTimeInfoDataStore
    {
        Core.Models.PosRealTimeInfo GetOrAddById(int id);
        IEnumerable<Core.Models.PosRealTimeInfo> GetAll();
    }
}