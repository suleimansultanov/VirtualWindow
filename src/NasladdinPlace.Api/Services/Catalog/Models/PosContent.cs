using NasladdinPlace.Api.Dtos.GoodCategory;
using NasladdinPlace.Api.Dtos.LabeledGood;
using System.Collections.Generic;

namespace NasladdinPlace.Api.Services.Catalog.Models
{
    public class PosContent
    {
        public GoodCategoryDto Category { get; set; }
        public ICollection<LabeledGoodWithImageDto> Goods { get; set; }
    }
}
