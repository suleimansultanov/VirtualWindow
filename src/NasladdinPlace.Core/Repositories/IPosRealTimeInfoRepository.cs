using NasladdinPlace.Core.Models;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosRealTimeInfoRepository
    {
        PosRealTimeInfo GetById(int id);
        List<PosRealTimeInfo> GetByIds(List<int> posIds);
        IEnumerable<PosRealTimeInfo> GetConnectedWithoutOrHavingVersionLessThan(string version);
        IEnumerable<PosRealTimeInfo> GetAll();
        IEnumerable<PosRealTimeInfo> GetConnected();
    }
}