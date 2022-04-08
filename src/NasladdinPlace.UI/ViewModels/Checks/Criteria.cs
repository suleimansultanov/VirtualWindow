using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NasladdinPlace.UI.ViewModels.Checks
{
    public class Criteria
    {
        public SelectList PlantSelectList { get; set; }

        [FromQuery(Name = "page")]
        public int Page { get; set; }

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; }

        [FromQuery(Name = "plantId")]
        public int? PlantId { get; set; }

        [FromQuery(Name = "dateTimeFrom")]
        public string DateTimeFrom { get; set; }

        [FromQuery(Name = "dateTimeUntil")]
        public string DateTimeUntil { get; set; }

        [FromQuery(Name = "userName")]
        public string UserName { get; set; }

        public Criteria()
        {
            Page = 1;
            PageSize = 50;
        }
    }
}
