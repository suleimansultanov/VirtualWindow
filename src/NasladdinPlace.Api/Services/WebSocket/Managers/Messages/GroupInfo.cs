namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages
{
    public class GroupInfo
    {
        public string Group { get; set; }
        public string ClientVersion { get; set; }

        public override string ToString()
        {
            return "##### GroupInfo ####\n" +
                   $"Group={Group}\n" +
                   $"ClientVersion={ClientVersion}\n" +
                   "###################";
        }
    }
}
