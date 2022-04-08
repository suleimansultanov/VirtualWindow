namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Factories
{
    public interface IActivityMessageFactory
    {
        EventMessage MakeEventMessage(string eventName, object body);
    }
}
