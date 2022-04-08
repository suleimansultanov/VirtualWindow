using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosAbnormalSensorMeasurementsRepository : Repository<PosAbnormalSensorMeasurement>, IPosAbnormalSensorMeasurementRepository
    {
        public PosAbnormalSensorMeasurementsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<PosAbnormalSensorMeasurement>> GetForDateRangeAsync(DateTime startDateTime, DateTime endDateTime)
        {
            return GetAll()
                .Where(ps => ps.DateMeasured >= startDateTime && ps.DateMeasured <= endDateTime)
                .ToListAsync();
        }

        public IQueryable<PosAbnormalSensorMeasurement> GetAllIncludingPos()
        {
            return GetAll()
                .Include(sm => sm.Pos);
        }
    }
}