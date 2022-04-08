using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Energy
{
    /// <summary>
    /// This dto is used only for the purpose of technical examination of Android/iOS candidates.
    /// </summary>
    public class EnergyOperationStatusDto
    {
        [Required]
        public int? Status { get; set; }
    }
}