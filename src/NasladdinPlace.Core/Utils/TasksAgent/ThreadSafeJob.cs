using System;
using FluentScheduler;

namespace NasladdinPlace.Core.Utils.TasksAgent
{
    public class ThreadSafeJob : IJob
    {
        private readonly Action _action;
        private readonly object _lock = new object();

        private bool _shuttingDown;

        public ThreadSafeJob(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                _action?.Invoke();
            }
        }

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }
        }
    }
}