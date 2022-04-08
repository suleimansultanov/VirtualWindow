namespace NasladdinPlace.Core.Services.OverdueGoods.Models
{
    public class GoodInstance
    {
        public string Name { get; }
        public string Price { get; }
        public string Label { get; }
        public string ExpirationDate { get; }
        public int PosId { get; }
        public string PosName { get; }

        public GoodInstance(
            string name, 
            string price, 
            string label, 
            string expirationDate,
            int posId,
            string posName)
        {
            Name = name.Replace('\n', ' ');
            Price = price;
            Label = label;
            ExpirationDate = expirationDate;
            PosId = posId;
            PosName = posName;
        }
    }
}