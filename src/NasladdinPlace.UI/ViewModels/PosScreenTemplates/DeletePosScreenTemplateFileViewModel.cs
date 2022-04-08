using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.PosScreenTemplates
{
    public class DeletePosScreenTemplateFileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }
    }
}