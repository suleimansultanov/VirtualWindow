using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Dtos.PosImage
{
    public class PosImageDto : IImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
    }
}