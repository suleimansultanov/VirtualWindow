using NasladdinPlace.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager
{
    public interface IPosTemperatureManager
    {
        double? GetPosTemperature(int posId);
        Task<IList<PosTemperature>> GetPointsOfSaleAbnormalTemperaturesAsync();
        IList<int> GetPosIdsToNotifyAboutNoTemperatureUpdates();
        Task DeletePosTemperaturesHistoricalDataAsync();
    }
}
