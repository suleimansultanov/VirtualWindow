using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Api.Dtos.OneCSync.Base;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class PosOperationDataDto : BaseOneCSyncDataDto
    {
        public ICollection<PosOperationDto> Sales { get; set; }

        public PosOperationDataDto()
        {
            Sales = new Collection<PosOperationDto>();
        }
    }
}