using System;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionKeepAliveManager
{
    public interface IWebSocketConnectionKeepAliveManager
    {
        void SendPeriodicKeepAliveMessages(TimeSpan updatePeriod);
    }
}
