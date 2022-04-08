using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosTemperatureRepository : IRepository<PosTemperature>
    {
        IEnumerable<PosTemperature> GetAverageOfPointsOfSale(TimeSpan measurementsPeriod);
        IEnumerable<PosTemperature> GetByPosId(int posId);
        PosTemperature GetAverageByPosId(int posId, TimeSpan measurementsPeriod);
        IEnumerable<PosTemperature> GetLatestByPosWithinPeriod(int posId, TimeSpan period);
        PosTemperature GetLatestByPos(int posId);
        IEnumerable<PosTemperature> GetAllPosTemperaturesOlderThanPeriod(TimeSpan period);
        IEnumerable<PosTemperature> GetPosTemperaturesWithinPeriod(int posId, DateTimeRange measurementsDateTimeRange);
    }
}
