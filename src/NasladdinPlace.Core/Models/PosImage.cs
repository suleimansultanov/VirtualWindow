namespace NasladdinPlace.Core.Models
{
    public class PosImage : Entity
    {
        public Pos Pos { get; private set; }

        public int PosId { get; private set; }
        public string ImagePath { get; private set; }

        public PosImage(int posId, string imagePath)
        {
            PosId = posId;
            ImagePath = imagePath;
        }
    }
}