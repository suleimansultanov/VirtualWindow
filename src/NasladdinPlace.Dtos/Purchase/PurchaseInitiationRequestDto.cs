using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Dtos.Purchase
{
    public class PurchaseInitiationRequestDto
    {
        [Required]
        public string QrCode { get; set; }
        public int? DoorPosition { get; set; }
    }
}
