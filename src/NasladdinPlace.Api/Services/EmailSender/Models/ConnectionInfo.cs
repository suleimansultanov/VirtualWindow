namespace NasladdinPlace.Api.Services.EmailSender.Models
{
    public sealed class ConnectionInfo
    {
        public string Url { get; }
        public int Port { get; }

        public ConnectionInfo(string url, int port)
        {
            Url = url;
            Port = port;
        }
    }
}