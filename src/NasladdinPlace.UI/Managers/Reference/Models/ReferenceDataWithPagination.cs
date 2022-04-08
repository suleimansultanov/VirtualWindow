using System.Collections.Generic;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    public class ReferenceDataWithPagination
    {
        public PaginationInfo PaginationInfo { get; set; }
        public List<BaseViewModel> Data { get; set; }
        public object CustomData { get; set; }
    }
}
