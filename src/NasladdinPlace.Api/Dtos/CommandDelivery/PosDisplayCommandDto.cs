using System;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.CommandDelivery
{
    [DataContract]
    public class PosDisplayCommandDto
    {
        [DataMember(Name = "commandId")]
        public Guid CommandId { get; set; }

        public PosDisplayCommandDto(Guid commandId)
        {
            CommandId = commandId;
        }
    }
}
