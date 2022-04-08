using System.Collections.Generic;
using NasladdinPlace.Core.Models.Reference;

namespace NasladdinPlace.UI.Controllers.Base.Models
{
    public class ReferenceLoadModal
    {
        public List<FilterItemModel> Filter { get; set; }
        public string ReferenceType { get; set; }
        public List<FilterItemModel> Context { get; set; }
        public PaginationInfo Pagination { get; set; }
    }
}
