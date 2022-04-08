using Microsoft.AspNetCore.Mvc.Rendering;
using NasladdinPlace.Utilities.ValidationAttributes.Date;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.LabeledGoods
{
    public class LabeledGoodsToGoodFormViewModel
    {
        public int PosId { get; set; }
        public SelectList PosSelectList { get; set; }

        public SelectList GoodSelectList { get; set; }

        public int GoodId { get; set; }

        [Date]
        [Required]
        public string ManufactureDate { get; set; }

        [FutureDate]
        [Required]
        public string ExpirationDate { get; set; }

        public IEnumerable<string> Labels { get; set; }

        public LabeledGoodsToGoodFormViewModel()
        {
            Labels = new Collection<string>();
        }
    }
}