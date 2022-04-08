using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.OneCSync.Base
{
    public abstract class BaseOneCSyncDataDto
    {
        public ICollection<PosSyncDto> PointOfSales { get; set; }
        public ICollection<GoodSyncDto> Goods { get; set; }

        protected BaseOneCSyncDataDto()
        {
            PointOfSales = new Collection<PosSyncDto>();
            Goods = new Collection<GoodSyncDto>();
        }
    }
}
