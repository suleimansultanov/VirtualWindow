using System.Collections.Generic;
using NasladdinPlace.Api.Dtos.Pos;

namespace NasladdinPlace.Api.Services.Catalog.Models
{
    public class CatalogPointOfSale
    {
        public PointOfSaleDto LastVisited { get; set; }
        public ICollection<PointOfSaleDto> Items { get; set; }
    }
}
