namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public interface IBytesConverter
    {
        T DeserializeObject<T>(byte[] bytes, int index, int count);
    }
}
