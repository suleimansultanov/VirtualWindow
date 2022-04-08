using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.UI.ViewModels.Shared
{
    public class ImageFormViewModel
    {
        public string ResourceName { get; set; }
        public int ResourceId { get; set; }
        public IFormFile ImageFile { get; set; }
        public string AspController { get; set; }
        public string AspAction { get; set; }
    }
}