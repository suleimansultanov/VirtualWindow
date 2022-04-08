using System.Collections.Generic;
using NasladdinPlace.Logging.Models;

namespace NasladdinPlace.Logging.Storage
{
    public interface ILogsStorage
    {
        void Add(Log log);
        IEnumerable<Log> GetAll();
        void Clear();
    }
}
