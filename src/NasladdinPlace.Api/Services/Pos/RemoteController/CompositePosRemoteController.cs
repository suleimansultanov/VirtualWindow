using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Pos.RemoteController
{
    public class CompositePosRemoteController : IPosRemoteController
    {
        private readonly IEnumerable<IPosRemoteController> _posRemoteControllers;
        
        public CompositePosRemoteController(IEnumerable<IPosRemoteController> posRemoteControllers)
        {
            if (posRemoteControllers == null)
                throw new ArgumentNullException(nameof(posRemoteControllers));
            
            _posRemoteControllers = posRemoteControllers.ToImmutableList();
        }
        
        public Task StartOperationInModeAsync(PosMode mode, PosDoorPosition doorPosition)
        {
            return PerformTask(c => c.StartOperationInModeAsync(mode, doorPosition));
        }

        public Task CompleteOperationAsync()
        {
            return PerformTask(c => c.CompleteOperationAsync());
        }

        public Task ContinueOperationAsync()
        {
            return PerformTask(c => c.ContinueOperationAsync());
        }

        public Task RequestAccountingBalancesAsync()
        {
            return PerformTask(c => c.RequestAccountingBalancesAsync());
        }

        public Task RequestAntennasOutputPowerAsync()
        {
            return PerformTask(c => c.RequestAntennasOutputPowerAsync());
        }

        public Task SetAntennasOutputPowerAsync(PosAntennasOutputPower antennasOutputPower)
        {
            return PerformTask(c => c.SetAntennasOutputPowerAsync(antennasOutputPower));
        }

        public Task RequestDoorsStateAsync()
        {
            return PerformTask(c => c.RequestDoorsStateAsync());
        }

        public Task RequestLogsAsync(PosLogType posLogType)
        {
            return PerformTask(c => c.RequestLogsAsync(posLogType));
        }

        private Task PerformTask(Func<IPosRemoteController, Task> performTask)
        {
            var remoteControllersTasks = new Collection<Task>();
            foreach (var posRemoteController in _posRemoteControllers)
            {
                remoteControllersTasks.Add(performTask(posRemoteController));
            }

            return Task.WhenAll(remoteControllersTasks);
        }
    }
}