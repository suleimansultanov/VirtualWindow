using System;

namespace NasladdinPlace.IntegrationTestsSmsReader.Common
{
    public class InboxAddress
    {
        public string Url { get; }
        public ushort Port { get; }

        public InboxAddress(string url, ushort port)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            Url = url;
            Port = port;
        }
    }
}