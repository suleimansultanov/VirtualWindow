using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.ViewModels
{
    public class PaymentConfirmationViewModel
    {
        [Required]
        public int? MD { get; set; }
        
        [Required]
        public string PaRes { get; set; }
    }
}