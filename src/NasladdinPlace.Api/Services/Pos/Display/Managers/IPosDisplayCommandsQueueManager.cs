using System;
using System.Collections.Generic;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;

namespace NasladdinPlace.Api.Services.Pos.Display.Managers
{
    public interface IPosDisplayCommandsQueueManager
    {
        List<PosDisplayCommand> GetAllCommands();

        void AddCommand(PosDisplayCommand command);

        void RemoveCommandById(Guid commandId);
    }
}
