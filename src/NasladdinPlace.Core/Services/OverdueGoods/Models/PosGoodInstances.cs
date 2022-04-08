using System.Collections.Generic;
using System.Collections.Immutable;

namespace NasladdinPlace.Core.Services.OverdueGoods.Models
{
    public class PosGoodInstances
    {
        public int PosId { get; }
        public string PosName { get; }
        public ICollection<GoodInstance> OverdueGoods { get; }

        public PosGoodInstances(int posId, string posName, IEnumerable<GoodInstance> overdueGoods)
        {
            PosId = posId;
            PosName = posName;
            OverdueGoods = overdueGoods.ToImmutableList();
        }

        public int Count => OverdueGoods.Count;
    }
}