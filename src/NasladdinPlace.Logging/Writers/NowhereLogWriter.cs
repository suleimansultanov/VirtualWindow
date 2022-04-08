using System.Threading.Tasks;
using NasladdinPlace.Logging.Models;

namespace NasladdinPlace.Logging.Writers
{
    public class NowhereLogWriter : ILogWriter
    {
        public Task Write(Log log)
        {
            return Task.CompletedTask;
        }
    }
}