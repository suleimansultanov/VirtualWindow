using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosDoorsStateRepository : IRepository<PosDoorsState>
    {
        PosDoorsState GetLatestByPosId(int posId);
        IEnumerable<PosDoorsState> GetAllDoorsStateOlderThanPeriod(TimeSpan period);
        IEnumerable<PosDoorsState> GetPosDoorsStatesOlderThanDate(int posId, DateTime date);
    }
}