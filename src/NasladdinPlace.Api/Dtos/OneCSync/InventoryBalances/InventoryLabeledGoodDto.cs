using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.OneCSync.InventoryBalances
{
    public class InventoryLabeledGoodDto
    {
        public int? PosId { get; set; }
        public int? GoodId { get; set; }
        public int InStock { get; set; }
        public ICollection<string> Labels { get; set; }

        public InventoryLabeledGoodDto()
        {
            Labels = new Collection<string>();
        }
    }
}
