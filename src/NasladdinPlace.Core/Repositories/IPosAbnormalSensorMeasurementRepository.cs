using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosAbnormalSensorMeasurementRepository : IRepository<PosAbnormalSensorMeasurement>
    {
        Task<List<PosAbnormalSensorMeasurement>> GetForDateRangeAsync(DateTime startDateTime, DateTime endDateTime);
        IQueryable<PosAbnormalSensorMeasurement> GetAllIncludingPos();
    }
}