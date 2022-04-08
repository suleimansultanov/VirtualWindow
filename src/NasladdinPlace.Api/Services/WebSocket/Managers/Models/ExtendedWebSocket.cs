using System;
using System.Net;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Models
{
    public sealed class ExtendedWebSocket : IEquatable<ExtendedWebSocket>
    {
        public readonly System.Net.WebSockets.WebSocket WebSocket;
        public readonly IPAddress IpAddress;
        private readonly object _lockObject = new object();

        public string Group
        {
            get
            {
                lock (_lockObject)
                {
                    return _group;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _group = value;
                }
            }
        }

        private string _group;

        public ExtendedWebSocket(System.Net.WebSockets.WebSocket webSocket, IPAddress ipAddress)
        {
            WebSocket = webSocket;
            IpAddress = ipAddress;
        }

        public override bool Equals(object obj)
        {
            return WebSocket.Equals(obj);
        }

        public bool Equals(ExtendedWebSocket other)
        {
            return WebSocket.Equals(other.WebSocket);
        }

        public override int GetHashCode()
        {
            return WebSocket.GetHashCode();
        }
    }
}
