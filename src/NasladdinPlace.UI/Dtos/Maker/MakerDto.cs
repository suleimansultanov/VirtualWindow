using System.ComponentModel.DataAnnotations;
using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Dtos.Maker
{
    public class MakerDto : ICommonHandbook
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Name { get; set; }
    }
}
