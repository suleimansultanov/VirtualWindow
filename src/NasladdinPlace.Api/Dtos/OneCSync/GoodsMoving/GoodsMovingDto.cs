using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.OneCSync.GoodsMoving
{
    public class GoodsMovingDto
    {
        public int DocumentId { get; set; }
        public DateTime DocumentCreatedDate { get; set; }
        public int PosId { get; set; }
        public ICollection<GoodsMovingTablePartDto> Goods { get; set; }

        public GoodsMovingDto()
        {
            Goods = new Collection<GoodsMovingTablePartDto>();
        }
    }
}
