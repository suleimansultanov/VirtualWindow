using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Services.Pos.Display.Agents.Models
{
    public class PosDisplayCommand
    {
        private readonly TimeSpan _commandExecutionRetryInterval;
        public  int PosId { get; private set; }

        public Guid CommandId { get; private set; }

        public PosDisplayContentType CommandContentType { get; private set; }

        public DateTime NextExecutionDateTime { get; private set; }

        public PosDisplayCommand(int posId,
                                 PosDisplayContentType contentType,
                                 TimeSpan commandExecutionRetryInterval,
                                 Guid commandId)
        {
            _commandExecutionRetryInterval = commandExecutionRetryInterval;
            PosId = posId;
            CommandId = commandId;
            CommandContentType = contentType;

            ScheduleNextExecutionDateTime();
        }

        public void ScheduleNextExecutionDateTime()
        {
            NextExecutionDateTime = DateTime.UtcNow.Add(_commandExecutionRetryInterval);
        }
    }
}
