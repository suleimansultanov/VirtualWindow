using NasladdinPlace.Core.Services.Pos.WebSocket.Models;
using NasladdinPlace.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution
{
    public class WsCommandsQueueProcessor : IWsCommandsQueueProcessor, IDisposable
    {
        public event EventHandler OnQueueProcessed;

        private readonly object _lockObject = new object();
        private readonly ConcurrentQueue<ControllerInvocationInfo> _commandsForExecutionQueue;
        private readonly Queue<string> _distictCommandsIdsQueue;
        private readonly EventWaitHandle _commandsForExecutionQueueWaitHandle;

        private readonly ILogger _logger;
        private readonly TimeSpan _commandWaitingTimeoutInMilliseconds;
        private readonly int _distinctCommandsIdCountLimit;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Thread _worker;

        public WsCommandsQueueProcessor(int distinctCommandsIdCountLimit, TimeSpan commandWaitingTimeoutInMilliseconds, ILogger logger)
        {
            _distinctCommandsIdCountLimit = distinctCommandsIdCountLimit;
            _commandWaitingTimeoutInMilliseconds = commandWaitingTimeoutInMilliseconds;
            _logger = logger;
            _commandsForExecutionQueue = new ConcurrentQueue<ControllerInvocationInfo>();
            _distictCommandsIdsQueue = new Queue<string>();
            _commandsForExecutionQueueWaitHandle = new AutoResetEvent(false);

            _cancellationTokenSource = new CancellationTokenSource();
            _worker = new Thread(async () => await ProcessCommands());
            _worker.Start();
        }

        public async Task EnqueueAndProcessAsync(ControllerInvocationInfo invocationInfo)
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    if (invocationInfo.Parameters.Length > 0 && invocationInfo.Parameters[0] != null)
                    {
                        var parameter = invocationInfo.Parameters[0];

                        var commandId = parameter
                            .GetType()
                            .GetProperty("CommandId", BindingFlags.Public | BindingFlags.Instance)
                            ?.GetValue(parameter, null).ToString();

                        if (commandId != null && _distictCommandsIdsQueue.Contains(commandId))
                            return;

                        _distictCommandsIdsQueue.Enqueue(commandId);

                        while (_distictCommandsIdsQueue.Count > _distinctCommandsIdCountLimit &&
                               _distictCommandsIdsQueue.TryDequeue(out var storedCommand)) ;
                    }

                    _commandsForExecutionQueue.Enqueue(invocationInfo);

                    if (!_commandsForExecutionQueueWaitHandle.SafeWaitHandle.IsClosed)
                        _commandsForExecutionQueueWaitHandle.Set();
                }
            });
        }

        private async Task ProcessCommands()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (_commandsForExecutionQueue.TryDequeue(out var commandInvocationInfo) && commandInvocationInfo != null)
                {
                    try
                    {
                        await (Task)commandInvocationInfo.Method.Invoke(commandInvocationInfo.Controller, commandInvocationInfo.Parameters);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"An error occured while executing command from commands queue. Exception {ex}");
                    }
                }
                else
                {
                    _commandsForExecutionQueueWaitHandle.WaitOne(_commandWaitingTimeoutInMilliseconds);

                    if (_commandsForExecutionQueue.IsEmpty)
                        OnQueueProcessed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _worker.Join();
                _commandsForExecutionQueueWaitHandle.Close();
            }
        }

        ~WsCommandsQueueProcessor()
        {
            Dispose(false);
        }
    }
}
