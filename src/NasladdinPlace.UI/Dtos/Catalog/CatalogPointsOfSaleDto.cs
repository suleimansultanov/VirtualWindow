using System.Collections.Generic;

namespace NasladdinPlace.UI.Dtos.Catalog
{
    public class CatalogPointsOfSaleDto
    {
        public PointOfSaleDto LastVisited { get; set; }
        public ICollection<PointOfSaleDto> Items { get; set; }
    }
}
