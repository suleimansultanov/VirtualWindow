namespace NasladdinPlace.Core.Models.Goods
{
    public class GoodImage : Entity
    {
        public Good Good { get; private set; }

        public int GoodId { get; private set; }
        public string ImagePath { get; private set; }

        public GoodImage(int goodId, string imagePath)
        {
            GoodId = goodId;
            ImagePath = imagePath;
        }

        public void SetImagePath(string imagePath)
        {
            ImagePath = imagePath;
        }
    }
}
