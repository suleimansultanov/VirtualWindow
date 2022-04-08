using System.Text.RegularExpressions;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public class StringUnescaper : IStringUnescaper
    {
        public string Unescape(string jsonString)
        {
            return Regex.Unescape(jsonString);
        }
    }
}
