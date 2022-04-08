using NasladdinPlace.Api.Dtos.OneCSync.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.OneCSync.GoodsMoving
{
    public class DocumentGoodsMovingDataDto : BaseOneCSyncDataDto
    {
        public ICollection<GoodsMovingDto> GoodsMoving { get; set; }

        public DocumentGoodsMovingDataDto()
        {
            GoodsMoving = new Collection<GoodsMovingDto>();
        }
    }
}
