namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public class IdFromGroupFetcher : IIdFromGroupFetcher
    {
        public int Fetch(string group, string groupPrefix)
        {
            if (string.IsNullOrWhiteSpace(group))
                return 0;

            return int.TryParse(group.Substring(groupPrefix.Length), out var posId) ? posId : 0;
        }
    }
}
