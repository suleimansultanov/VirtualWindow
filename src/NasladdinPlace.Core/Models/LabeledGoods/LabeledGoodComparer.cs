using System;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Models.LabeledGoods
{
    public class LabeledGoodComparer : IEqualityComparer<LabeledGood>
    {
        public bool Equals(LabeledGood x, LabeledGood y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return x.Good.Id == y.Good.Id && x.Price == y.Price;
        }

        public int GetHashCode(LabeledGood obj)
        {
            return obj.Good.Id.GetHashCode();
        }
    }
}
