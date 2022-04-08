using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.Dtos.Pos
{
    public class PosVersionDto
    {
        [Required]
        public string Value { get; set; }

        public PosVersionDto()
        {
            // intentionally left empty
        }

        public PosVersionDto(string value)
        {
            Value = value;
        }
    }
}