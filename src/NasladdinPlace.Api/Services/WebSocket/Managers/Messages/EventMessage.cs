using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Messages
{
    [DataContract]
    public class EventMessage
    {
        [DataMember(Name = "M")]
        public string Event { get; set; }

        [DataMember(Name = "H")]
        public string Activity { get; set; }

        [DataMember(Name = "A")]
        public object Body { get; set; }
    }
}
