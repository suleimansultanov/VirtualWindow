namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public interface IIdFromGroupFetcher
    {
        int Fetch(string group, string groupPrefix);
    }
}
