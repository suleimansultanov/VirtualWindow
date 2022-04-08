using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.PosScreenTemplates
{
    public class CreatePosScreenTemplateViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}