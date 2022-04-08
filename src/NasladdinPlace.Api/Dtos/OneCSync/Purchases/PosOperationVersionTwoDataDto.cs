using NasladdinPlace.Api.Dtos.OneCSync.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class PosOperationVersionTwoDataDto : BaseOneCSyncDataDto
    {
        public ICollection<PosOperationVersionTwoDto> Sales { get; set; }

        public PosOperationVersionTwoDataDto()
        {
            Sales = new Collection<PosOperationVersionTwoDto>();
        }
    }
}
