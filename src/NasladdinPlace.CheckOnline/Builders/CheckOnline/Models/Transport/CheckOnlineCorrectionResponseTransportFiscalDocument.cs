using System.Collections.Generic;
using Newtonsoft.Json;

namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport
{
    public class CheckOnlineCorrectionResponseTransportFiscalDocument
    { 
       [JsonProperty("Value")]
       public List<CheckOnlineFiscalDocumentTag> Tags { get; set; }
    }
}
