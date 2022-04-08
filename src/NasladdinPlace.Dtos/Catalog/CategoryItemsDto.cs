using System;
using System.Collections.Generic;
using System.Text;

namespace NasladdinPlace.Dtos.Catalog
{
    public class CategoryItemsDto
    {
        public int PosId { get; set; }
        public int CategoryId { get; set; }
        public byte Page { get; set; }
    }
}
