using NasladdinPlace.Api.Dtos.Good;
using NasladdinPlace.Api.Dtos.GoodCategory;
using System.Collections.Generic;

namespace NasladdinPlace.Api.Services.Catalog.Models
{
    public class VirtualPosContent
    {
        public GoodCategoryDto Category { get; set; }
        public ICollection<GoodWithImageAndNutrients> Goods { get; set; }
    }
}
