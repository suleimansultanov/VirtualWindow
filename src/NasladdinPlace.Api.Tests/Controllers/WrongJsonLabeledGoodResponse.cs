namespace NasladdinPlace.Api.Tests.Controllers
{
    /// <summary>
    /// Прежде, чем менять, оповестить разработчика izitrack
    /// </summary>
    public class WrongJsonLabeledGoodResponse : JsonLabeledGoodResponse
    {
        public int NonexistedProperty { get; set; }
    }
}
