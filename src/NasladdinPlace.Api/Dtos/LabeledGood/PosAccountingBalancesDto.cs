using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Api.Dtos.Pos;

namespace NasladdinPlace.Api.Dtos.LabeledGood
{
    public class PosAccountingBalancesDto : BasePosWsMessageDto
    {
        public ICollection<string> Labels { get; set; }

        public Guid CommandId { get; set; }

        public PosAccountingBalancesDto()
        {
            Labels = new Collection<string>();
        }
    }
}
