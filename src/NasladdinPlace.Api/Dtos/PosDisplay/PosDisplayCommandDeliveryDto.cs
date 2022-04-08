using System;
using NasladdinPlace.Api.Dtos.Pos;

namespace NasladdinPlace.Api.Dtos.PosDisplay
{
    public class PosDisplayCommandDeliveryDto : BasePosWsMessageDto
    {
        public Guid CommandId { get; set; }
    }
}
