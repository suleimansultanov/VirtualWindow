using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;
using System.Collections.Generic;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.UI.ViewModels.PosScreenTemplates
{
    public class PosScreenTemplateFormViewModel
    {
        public int Id { get; set; }

        [LocalizedRequired]
        [LocalizedStringLength(255)]
        public string Name { get; set; }

        public List<PosBasicInfoViewModel> PointsOfSale { get; set; }
        public IReadOnlyCollection<string> TemplateFiles { get; set; }
        public IReadOnlyCollection<ScreenResolution> ScreenResolutions { get; set; }

        public PosScreenTemplateFormViewModel()
        {
            PointsOfSale = new List<PosBasicInfoViewModel>();
            TemplateFiles = new List<string>();
            ScreenResolutions = new List<ScreenResolution>();
        }
    }
}
