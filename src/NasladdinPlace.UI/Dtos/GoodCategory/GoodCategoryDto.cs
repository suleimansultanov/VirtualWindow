using System.ComponentModel.DataAnnotations;
using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Dtos.GoodCategory
{
    public class GoodCategoryDto : ICommonHandbook
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}