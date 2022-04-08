using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Core.Models;
using System;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.GroupHandlers.Handlers
{
    public class PosGroupHandler : BasePosGroupHandler
    {
        public PosGroupHandler(
            IServiceProvider serviceProvider, GroupInfo groupInfo)
            : base(serviceProvider, groupInfo)
        {
        }

        protected override string GetGroupPrefix()
        {
            return Groups.Pos;
        }

        protected override void UpdatePosRealTimeInfo(PosRealTimeInfo realTimeInfo)
        {
            realTimeInfo.Version = GroupInfo.ClientVersion;
        }
    }
}
