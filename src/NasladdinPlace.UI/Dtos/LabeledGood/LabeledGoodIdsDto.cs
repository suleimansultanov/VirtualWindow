using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.UI.Dtos.LabeledGood
{
    public class LabeledGoodIdsDto
    {
        public ICollection<int> Values { get; set; }

        public LabeledGoodIdsDto()
        {
            Values = new Collection<int>();
        }
    }
}