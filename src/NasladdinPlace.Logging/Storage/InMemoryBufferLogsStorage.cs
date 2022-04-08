using System.Collections.Generic;
using NasladdinPlace.Logging.Models;
using NasladdinPlace.Utilities.Buffer;

namespace NasladdinPlace.Logging.Storage
{
    public class BufferLogsStorage : ILogsStorage
    {
        private readonly IBuffer<Log> _buffer;

        public BufferLogsStorage(IBuffer<Log> buffer)
        {
            _buffer = buffer;
        }

        public void Add(Log log)
        {
            _buffer.Add(log);
        }

        public IEnumerable<Log> GetAll()
        {
            return _buffer.GetAll();
        }

        public void Clear()
        {
            _buffer.Clear();
        }
    }
}
