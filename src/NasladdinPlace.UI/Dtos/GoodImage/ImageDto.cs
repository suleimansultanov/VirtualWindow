using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Dtos.GoodImage
{
    public class ImageDto : IImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
    }
}
