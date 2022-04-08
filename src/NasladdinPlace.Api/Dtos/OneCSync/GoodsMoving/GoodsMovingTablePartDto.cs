using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.OneCSync.GoodsMoving
{
    public class GoodsMovingTablePartDto
    {
        public int? GoodId { get; set; }
        public int Income { get; set; }
        public int Outcome { get; set; }
        public ICollection<string> IncomeLabels { get; set; }
        public ICollection<string> OutcomeLabels { get; set; }

        public GoodsMovingTablePartDto()
        {
            IncomeLabels = new Collection<string>();
            OutcomeLabels = new Collection<string>();
        }
    }
}
