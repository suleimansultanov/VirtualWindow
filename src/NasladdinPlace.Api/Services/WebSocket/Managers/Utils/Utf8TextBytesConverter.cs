using System.Text;
using Newtonsoft.Json;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public class Utf8JsonStringBytesConverter : IBytesConverter
    {
        private readonly IStringUnescaper _stringUnescaper;

        public Utf8JsonStringBytesConverter(IStringUnescaper stringUnescaper)
        {
            _stringUnescaper = stringUnescaper;
        }

        public T DeserializeObject<T>(byte[] bytes, int index, int count)
        {
            var text = Encoding.UTF8.GetString(bytes, index, count);
            return JsonConvert.DeserializeObject<T>(_stringUnescaper.Unescape(text));
        }
    }
}
