using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.DAL.DataStore.PosRealTimeInfo
{
    public class StubPosRealmInfoDataStore : IPosRealTimeInfoDataStore
    {
        public Core.Models.PosRealTimeInfo GetOrAddById(int id)
        {
            return new Core.Models.PosRealTimeInfo(id);
        }

        public IEnumerable<Core.Models.PosRealTimeInfo> GetAll()
        {
            return Enumerable.Empty<Core.Models.PosRealTimeInfo>();
        }
    }
}