using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;

namespace NasladdinPlace.Api.Services.Pos.Display.Managers
{
    public class PosDisplayCommandsQueueManager : IPosDisplayCommandsQueueManager
    {
        private readonly object _queueLock = new object();
        
        private readonly List<PosDisplayCommand> _posDisplayCommands = new List<PosDisplayCommand>();

        public List<PosDisplayCommand> GetAllCommands()
        {
            lock (_queueLock)
            {
                return _posDisplayCommands.ToList();
            }
        }

        public void AddCommand(PosDisplayCommand command)
        {
            lock (_queueLock)
            {
                var obsoletePosCommands = _posDisplayCommands.Where(p => p.PosId == command.PosId).ToList();
                foreach (var obsoletePosCommand in obsoletePosCommands)
                {
                    _posDisplayCommands.Remove(obsoletePosCommand);
                }

                _posDisplayCommands.Add(command);
            }
        }

        public void RemoveCommandById(Guid commandId)
        {
            lock (_queueLock)
            {
                var command = _posDisplayCommands.FirstOrDefault(p => p.CommandId == commandId);
                if (command == null)
                    return;

                _posDisplayCommands.Remove(command);
            }
        }
    }
}
