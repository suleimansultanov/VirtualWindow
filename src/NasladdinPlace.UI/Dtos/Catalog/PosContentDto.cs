using System.Collections.Generic;
using NasladdinPlace.UI.Dtos.GoodCategory;

namespace NasladdinPlace.UI.Dtos.Catalog
{
    public class PosContentDto
    {
        public GoodCategoryDto Category { get; set; }
        public ICollection<LabeledGoodWithImageDto> Goods { get; set; }
    }
}
