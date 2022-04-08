using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Api.Dtos.OneCSync.Base;

namespace NasladdinPlace.Api.Dtos.OneCSync.InventoryBalances
{
    public class InventoryBalanceDataDto : BaseOneCSyncDataDto
    {
        public ICollection<InventoryLabeledGoodDto> InventoryBalances { get; set; }

        public InventoryBalanceDataDto()
        {
            InventoryBalances = new Collection<InventoryLabeledGoodDto>();
        }
    }
}
