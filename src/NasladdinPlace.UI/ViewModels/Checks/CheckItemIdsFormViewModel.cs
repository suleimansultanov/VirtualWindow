using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Checks
{
    public class CheckItemIdsFormViewModel
    {
        [Required]
        public ICollection<int> CheckItemsIds { get; set; }
    }
}
