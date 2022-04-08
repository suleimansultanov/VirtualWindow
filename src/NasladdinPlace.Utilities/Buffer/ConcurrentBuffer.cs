using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NasladdinPlace.Utilities.Buffer
{
    public class ConcurrentBuffer<T> : IBuffer<T>
    {
        private readonly int _recordsNumberToKeep;
        private readonly object _lockObject = new object();

        private ConcurrentQueue<T> _bufferQueue;

        public ConcurrentBuffer(int recordsNumberToKeep)
        {        
            if (recordsNumberToKeep < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(recordsNumberToKeep),
                    recordsNumberToKeep,
                    $"The {nameof(recordsNumberToKeep)} cannot be less than zero."
                );

            _recordsNumberToKeep = recordsNumberToKeep;
            _bufferQueue = new ConcurrentQueue<T>();
        }

        public void Add(T element)
        {
            lock (_lockObject)
            {
                if (_bufferQueue.Count == _recordsNumberToKeep)
                    _bufferQueue.TryDequeue(out T _);

                _bufferQueue.Enqueue(element);
            }
        }

        public IEnumerable<T> GetAll()
        {
            return _bufferQueue.ToImmutableList();
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _bufferQueue = new ConcurrentQueue<T>();
            }
        }
    }
}
