using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheckGoodsStatistics
    {
        public int CheckItemsRefunded { get; }
        public int CheckItemsModifiedByAdmin { get; }
        public int CheckItemsDeleted { get; }

        public DetailedCheckGoodsStatistics(IReadOnlyCollection<DetailedCheckGood> detailedCheckGoods)
        {
            CheckItemsRefunded = detailedCheckGoods.Select(cg => cg.Instances.Count(i => i.Status == CheckItemStatus.Refunded)).Sum();
            CheckItemsModifiedByAdmin = detailedCheckGoods.Select(cg => cg.Instances.Count(i => i.IsModifiedByAdmin)).Sum();
            CheckItemsDeleted = detailedCheckGoods.Select(cg => cg.Instances.Count(i => i.Status == CheckItemStatus.Deleted)).Sum();
        }
    }
}