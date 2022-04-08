namespace NasladdinPlace.Api.Dtos.Good
{
    public class GoodPriceInPosDto
    {
        public int PosId { get; set; }
        public int GoodId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Price { get; set; }
    }
}