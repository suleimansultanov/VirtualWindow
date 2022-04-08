using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace NasladdinPlace.UI.ViewModels.PointsOfSale
{
    public class PosDoorViewModel
    {
        public bool IsLeftDoor { get; set; }
        public int PosId { get; set; }
        public IEnumerable<SelectListItem> Modes { get; set; }
    }
}
