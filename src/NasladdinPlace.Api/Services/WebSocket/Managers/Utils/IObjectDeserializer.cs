using System;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public interface IObjectDeserializer
    {
        T Deserialize<T>(object o) where T: class;
        object Deserialize(object o, Type type);
    }
}
