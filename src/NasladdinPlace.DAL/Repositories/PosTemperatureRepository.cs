using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosTemperatureRepository : Repository<PosTemperature>, IPosTemperatureRepository
    {

        public PosTemperatureRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<PosTemperature> GetAverageOfPointsOfSale(TimeSpan measurementsPeriod)
        {
            var obsoleteTemperaturesEndTime = GetObsoleteTemperaturesEndTime(measurementsPeriod);

            var averageTemperatures = GetAll()
                .Where(t => t.DateCreated >= obsoleteTemperaturesEndTime)
                .GroupBy(t => t.PosId)
                .Select(t => new PosTemperature(t.Key, t.Average(a => a.Temperature)));

            return averageTemperatures;
        }

        public IEnumerable<PosTemperature> GetByPosId(int posId)
        {
            return GetAll().Where(t => t.PosId == posId);
        }

        public PosTemperature GetAverageByPosId(int posId, TimeSpan measurementsPeriod)
        {
            var emptyPosTemperature = PosTemperature.EmptyOfPos(posId);

            var posTemperatures = GetByPosId(posId);
            
            if (!posTemperatures.Any())
                return emptyPosTemperature;

            var obsoleteTemperaturesEndTime = GetObsoleteTemperaturesEndTime(measurementsPeriod);

            var actualPosTemperatures = posTemperatures
                .Where(t => t.DateCreated >= obsoleteTemperaturesEndTime)
                .ToImmutableList();
        
            return !actualPosTemperatures.Any()
                ? emptyPosTemperature
                : new PosTemperature(posId, actualPosTemperatures.Average(v => v.Temperature));
        }

        public IEnumerable<PosTemperature> GetLatestByPosWithinPeriod(int posId, TimeSpan period)
        {
            var obsoleteTemperaturesEndDate = DateTime.UtcNow.Add(-period);

            return GetAll().Where(t => t.DateCreated >= obsoleteTemperaturesEndDate && t.PosId == posId);
        }

        public PosTemperature GetLatestByPos(int posId)
        {
            var posTemperatures = GetByPosId(posId);

            var latestTemperature = PosTemperature.EmptyOfPos(posId);

            if (posTemperatures.Any())
            {
                latestTemperature = posTemperatures.OrderByDescending(t => t.DateCreated).First();
            }

            return latestTemperature;
        }

        public IEnumerable<PosTemperature> GetAllPosTemperaturesOlderThanPeriod(TimeSpan period)
        {
            var obsoleteDataDateCreated = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(DateTime.UtcNow.Date.Add(-period));
            return GetAll().Where(p => p.DateCreated <= obsoleteDataDateCreated);
        }

        public  IEnumerable<PosTemperature> GetPosTemperaturesWithinPeriod(int posId, DateTimeRange measurementsDateTimeRange)
        {
            return GetAll().Where(t => t.PosId == posId && t.DateCreated >= measurementsDateTimeRange.Start && t.DateCreated <= measurementsDateTimeRange.End);
        }

        private DateTime GetObsoleteTemperaturesEndTime(TimeSpan measurementsPeriod)
        {
            return DateTime.UtcNow.Add(-measurementsPeriod);
        }
    }
}