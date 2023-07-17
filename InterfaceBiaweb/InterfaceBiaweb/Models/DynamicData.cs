using SigeaciInterface.Helpers;
using System.Text.Json.Serialization;

namespace SigeaciInterface.Models
{
    public class DynamicData
    {
        [JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
        public Dictionary<string, object> Data { get; set; }
    }
}
