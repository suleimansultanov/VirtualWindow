namespace NasladdinPlace.Api.Tests.Controllers
{
    /// <summary>
    /// Прежде, чем менять, оповестить разработчика izitrack
    /// </summary>
    public class JsonGoodModel
    {
        public object Maker { get; set; }
        public int Id { get; set; }
        public int MakerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Volume { get; set; }
        public object NetWeight { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public object CheckInfo { get; set; }
    }
}
