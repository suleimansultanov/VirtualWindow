using NasladdinPlace.Core.Enums;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Pos
{
    [DataContract]
    public class PosOperationInitiationParamsDto
    {
        [DataMember(Name = "doorPosition")]
        public PosDoorPosition DoorPosition { get; }

        [DataMember(Name = "posMode")]
        public PosMode PosMode { get; }

        public PosOperationInitiationParamsDto(PosDoorPosition doorPosition, PosMode posMode)
        {
            DoorPosition = doorPosition;
            PosMode = posMode;
        }
    }
}