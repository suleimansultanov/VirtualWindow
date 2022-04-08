using System;
using Newtonsoft.Json;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public class ObjectDeserializer : IObjectDeserializer
    {
        public T Deserialize<T>(object o) where T: class
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(o);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object Deserialize(object o, Type type)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(o);
                return JsonConvert.DeserializeObject(jsonString, type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
