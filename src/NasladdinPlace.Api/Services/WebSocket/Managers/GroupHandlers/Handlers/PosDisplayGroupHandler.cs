using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using System;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Handlers
{
    public class PosDisplayGroupHandler : BasePosGroupHandler
    {
        public PosDisplayGroupHandler(
            IServiceProvider serviceProvider,
            GroupInfo groupInfo) 
            : base(serviceProvider, groupInfo)
        {
        }

        protected override string GetGroupPrefix()
        {
            return Groups.PosDisplay;
        }
    }
}
