using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosDoorsStateRepository : Repository<PosDoorsState>, IPosDoorsStateRepository
    {
        public PosDoorsStateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public PosDoorsState GetLatestByPosId(int posId)
        {
            return GetAll().OrderByDescending(p => p.DateCreated).FirstOrDefault(p => p.PosId == posId);
        }

        public IEnumerable<PosDoorsState> GetAllDoorsStateOlderThanPeriod(TimeSpan period)
        {
            var obsoletDataDateCreated = UtcMoscowDateTimeConverter.ConvertToUtcDateTime(DateTime.UtcNow.Date.Add(-period));
            return GetAll().Where(p => p.DateCreated <= obsoletDataDateCreated);
        }

        public IEnumerable<PosDoorsState> GetPosDoorsStatesOlderThanDate(int posId, DateTime date)
        {
            return GetAll().Where(t => t.PosId == posId && t.DateCreated <= date);
        }
    }
}