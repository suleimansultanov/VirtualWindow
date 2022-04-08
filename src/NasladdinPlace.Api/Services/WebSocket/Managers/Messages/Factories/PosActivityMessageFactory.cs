namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Factories
{
    public class PosActivityMessageFactory : IActivityMessageFactory
    {
        private const string PosActivityName = "PlantHub";

        public EventMessage MakeEventMessage(string eventName, object body)
        {
            return new EventMessage
            {
                Activity = PosActivityName,
                Body = body,
                Event = eventName
            };
        }
    }
}
