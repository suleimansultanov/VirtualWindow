using System.Collections.Generic;

namespace NasladdinPlace.Api.Tests.Scenarios.OverdueGoods.Models
{
    public class ExpectedOverdueGoodsInfoInstance
    {
        public static ExpectedOverdueGoodsInfoInstance DefaultInstance()
        {
            return new ExpectedOverdueGoodsInfoInstance(0, new Dictionary<int, int>());
        }

        public int GroupedCount { get; set; }
        public Dictionary<int, int> InstancesCountByPos { get; set; }


        public ExpectedOverdueGoodsInfoInstance(int groupedCount, Dictionary<int, int> instancesCountByPos)
        {
            GroupedCount = groupedCount;
            InstancesCountByPos = instancesCountByPos;
        }
    }
}