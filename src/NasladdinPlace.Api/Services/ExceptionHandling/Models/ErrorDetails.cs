using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NasladdinPlace.Api.Services.ExceptionHandling.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(this, serializerSettings);
        }
    }
}