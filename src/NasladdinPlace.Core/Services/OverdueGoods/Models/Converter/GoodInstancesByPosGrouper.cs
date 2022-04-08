using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NasladdinPlace.Core.Services.OverdueGoods.Models.Converter
{
    public class GoodInstancesByPosGrouper : IGoodInstancesByPosGrouper
    {
        public IEnumerable<PosGoodInstances> Group(IEnumerable<GoodInstance> goodInstances)
        {
            var posInstancesByPosIdDictionary = new Dictionary<int, ICollection<GoodInstance>>();
            var posNameByPosId = new Dictionary<int, string>();

            foreach (var goodInstance in goodInstances)
            {
                if (!posInstancesByPosIdDictionary.ContainsKey(goodInstance.PosId))
                {
                    posNameByPosId[goodInstance.PosId] = goodInstance.PosName;
                    posInstancesByPosIdDictionary[goodInstance.PosId] = new Collection<GoodInstance> { goodInstance };
                }
                else
                {
                    posInstancesByPosIdDictionary[goodInstance.PosId].Add(goodInstance);
                }
            }
            
            return posInstancesByPosIdDictionary.Select(e => 
                new PosGoodInstances(e.Key, posNameByPosId[e.Key], e.Value));
        }
    }
}