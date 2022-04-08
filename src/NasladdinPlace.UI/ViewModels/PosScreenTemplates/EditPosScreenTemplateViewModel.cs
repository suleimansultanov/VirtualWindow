using NasladdinPlace.UI.ViewModels.PointsOfSale;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.PosScreenTemplates
{
    public class EditPosScreenTemplateViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<PosBasicInfoViewModel> PointsOfSale { get; set; }

        public EditPosScreenTemplateViewModel()
        {
            PointsOfSale = new List<PosBasicInfoViewModel>();
        }
    }
}