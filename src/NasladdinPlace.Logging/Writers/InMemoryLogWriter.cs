using System.Threading.Tasks;
using NasladdinPlace.Logging.Models;
using NasladdinPlace.Logging.Storage;

namespace NasladdinPlace.Logging.Writers
{
    public class InMemoryLogWriter : ILogWriter
    {
        private readonly ILogsStorage _logsStorage;

        public InMemoryLogWriter(ILogsStorage logsStorage)
        {
            _logsStorage = logsStorage;
        }

        public Task Write(Log log)
        {
            _logsStorage.Add(log);

            return Task.CompletedTask;
        }
    }
}
