using System.Collections.Generic;
using System.Net;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class PosConnectionInfo
    {
        public PosConnectionStatus ConnectionStatus { get; }
        public IEnumerable<IPAddress> IpAddresses { get; }

        public PosConnectionInfo(PosConnectionStatus connectionStatus, IEnumerable<IPAddress> ipAddresses)
        {
            ConnectionStatus = connectionStatus;
            IpAddresses = ipAddresses ?? new List<IPAddress>();
        }
    }
}
