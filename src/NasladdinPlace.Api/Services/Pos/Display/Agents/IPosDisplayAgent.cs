using System;
using System.Collections.Generic;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;

namespace NasladdinPlace.Api.Services.Pos.Display.Agents
{
    public interface IPosDisplayAgent
    {
        event EventHandler<int> OnPerformSwitchingToDisconnectPage;

        event EventHandler<List<PosDisplayCommand>> OnPosDisplayActionExecuted;

        void StartWaitingSwitchingToDisconnect(int posId, TimeSpan disconnectAfterTime);

        void StopWaitingSwitchingToDisconnect(int posId);

        void StartCheckCommandsForRetrySend(TimeSpan checkInterval);

        void StopCheckCommandsForRetrySend(Guid commandId);
    }
}
