namespace NasladdinPlace.Api.Tests.Controllers
{
    /// <summary>
    /// Прежде, чем менять, оповестить разработчика izitrack
    /// </summary>
    public class JsonLabeledGoodResponse
    {
        public long? ManufactureDate { get; set; }
        public long? ExpirationDate { get; set; }
        public bool CanBeDeleted { get; set; }
        public string CannotBeDeletedReason { get; set; }
        public JsonGoodModel Good { get; set; }
        public int Id { get; set; }
        public string Label { get; set; }
        public int? GoodId { get; set; }
        public int? PosId { get; set; }
        public int? PosOperationId { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; }
        public int? CurrencyId { get; set; }
    }
}
