using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Checks
{
    public class CheckItemAdditionFormViewModel
    {
        [Required]
        public int? LabelGoodId { get; set; }

        public SelectList LabeledGoodItems { get; set; }
    }
}
